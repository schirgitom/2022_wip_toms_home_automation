using Context.DAL.Alarming;
using Context.DAL.Data;
using Context.DAL.Data.Sources;
using Context.Drivers;
using Context.UnitOfWork;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Context
{
    public class DataAcquistion
    {
        protected ILogger log = Logger.ContextLog<DataAcquistion>();
        MongoDBUnitOfWork MongoUoW = null;
        InfluxDBUnitOfWork InfluxDBUnitOfWork = null;
        List<Driver> Drivers = new List<Driver>();
        public DataAcquistion(MongoDBUnitOfWork MongoUnitOfWork, InfluxDBUnitOfWork influxDBUnitOfWork)
        {
            MongoUoW = MongoUnitOfWork;
            InfluxDBUnitOfWork = influxDBUnitOfWork;
        }

        public async Task Start()
        {
            List<MQTTDatasource> mqtt = await MongoUoW.DataSources.GetMQTTDataSources();

            if (mqtt != null)
            {
                foreach (MQTTDatasource ds in mqtt)
                {
                    MQTTDriver driver = new MQTTDriver(ds);
                    Drivers.Add(driver);
                }
            }

            List<ModbusDatasource> modbus = await MongoUoW.DataSources.GetModbusDataSources();

            if (modbus != null)
            {
                foreach (ModbusDatasource ds in modbus)
                {
                    ModbusDriver driver = new ModbusDriver(ds);
                    Drivers.Add(driver);
                }
            }

            foreach (Driver driver in Drivers)
            {
                StartDriver(driver);
            }

            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            Save();

        }

        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Save();
        }

        private async Task StartDriver(Driver driver)
        {
            await driver.Create();
            await driver.Connect();
        }


        private async Task Save()
        {

            foreach (Driver driver in Drivers)
            {
                log.Information("Saving " + driver.Name);
                await driver.Read();
                Dictionary<String, List<Measurement>> numericdata = await driver.NumericData();
                Dictionary<String, List<Measurement>> binarydata = await driver.BinaryData();

                List<Measurement> allmeasurements = new List<Measurement>();

                foreach (KeyValuePair<String, List<Measurement>> meas in binarydata)
                {
                    allmeasurements.AddRange(meas.Value);
                }

                foreach (KeyValuePair<String, List<Measurement>> meas in numericdata)
                {
                    allmeasurements.AddRange(meas.Value);
                }



                InfluxDBUnitOfWork.Repository.InsertManyAsync(InfluxDBUnitOfWork.ConvertToNumericList(numericdata));
                InfluxDBUnitOfWork.Repository.InsertManyAsync(InfluxDBUnitOfWork.ConvertToBinaryList(binarydata));

                // CheckAlarm();

                await AlarmCheck(allmeasurements);


                driver.Clear();
                log.Information("Saving " + driver.Name + " finished");


            }
        }

        public async Task AlarmCheck(List<Measurement> measurements)
        {
            if (measurements != null)
            {
                foreach (Measurement measurement in measurements)
                {
                    DataPoint pt = measurement.DataPointObject;
                    String datapoint = pt.DatabaseName;
                    if (pt != null)
                    {
                        if (pt.AlarmThresholds != null && pt.AlarmThresholds.Count > 0)
                        {
                            AlarmThreshold warning = null;
                            AlarmThreshold alarm = null;
                            AlarmThreshold trip = null;

                            if (pt.AlarmThresholds.ContainsKey(AlarmType.Alarm.ToString()))
                            {
                                alarm = pt.AlarmThresholds[AlarmType.Alarm.ToString()];
                            }

                            if (pt.AlarmThresholds.ContainsKey(AlarmType.Warning.ToString()))
                            {
                                warning = pt.AlarmThresholds[AlarmType.Warning.ToString()];
                            }

                            if (pt.AlarmThresholds.ContainsKey(AlarmType.Trip.ToString()))
                            {
                                trip = pt.AlarmThresholds[AlarmType.Trip.ToString()];
                            }

                            FireStatus hasWarning = await CheckAgainst(warning, measurement);
                            FireStatus hasAlarm = await CheckAgainst(alarm, measurement);
                            FireStatus hasTrip = await CheckAgainst(trip, measurement);



                            if (hasTrip == FireStatus.Fire)
                            {
                                await DeactivateFormer(datapoint, AlarmType.Warning);
                                await DeactivateFormer(datapoint, AlarmType.Alarm);
                                await Fire(pt, trip);
                            }
                            else if (hasAlarm == FireStatus.Fire)
                            {
                                await DeactivateFormer(datapoint, AlarmType.Warning);
                                await DeactivateFormer(datapoint, AlarmType.Trip);
                                await Fire(pt, alarm);
                            }
                            else if (hasWarning == FireStatus.Fire)
                            {
                                await DeactivateFormer(datapoint, AlarmType.Alarm);
                                await DeactivateFormer(datapoint, AlarmType.Trip);
                                await Fire(pt, warning);
                            }
                            else if (hasWarning == FireStatus.NotFire && hasTrip == FireStatus.NotFire && hasAlarm == FireStatus.NotFire)
                            {
                                await DeactivateFormer(datapoint, AlarmType.Alarm);
                                await DeactivateFormer(datapoint, AlarmType.Trip);
                                await DeactivateFormer(datapoint, AlarmType.Warning);
                            }
                        }
                    }
                }
            }
        }

        private async Task DeactivateFormer(String dp, AlarmType alarmType)
        {
            AlarmListEntry alarme = await MongoUoW.AlarmList.IsAlarmActiveForDataPointAndLevel(dp, alarmType);

            if (alarme != null)
            {
                alarme.AlarmStatus = AlarmStatus.Deactive;
                alarme.DeactiveDate = DateTime.Now;

                await MongoUoW.AlarmList.UpdateOneAsync(alarme);
            }
        }

        private async Task Fire(DataPoint dp, AlarmThreshold th)
        {
            AlarmListEntry entry = new AlarmListEntry();
            entry.AlarmText = th.Message;
            entry.ActiveDate = DateTime.Now;
            entry.AlarmStatus = AlarmStatus.Active;
            entry.AcknowledgeStatus = AcknowledgeStatus.NotAcknowledged;
            entry.AlarmType = th.AlarmType;
            entry.DataPoint = dp;
            await MongoUoW.AlarmList.InsertOneAsync(entry);
        }



        private FireStatus GetStatus(AlarmThreshold th, AlarmListEntry entry, Measurement m)
        {
            if (th.AlarmType == AlarmType.Trip && (entry.AlarmType == AlarmType.Warning || entry.AlarmType == AlarmType.Alarm))
            {
                log.Debug("Value " + m.DataPointObject.DatabaseName + ": in Alarmlist with lower level - firing");
                return FireStatus.Fire;
            }
            else if (th.AlarmType == AlarmType.Alarm && entry.AlarmType == AlarmType.Warning)
            {
                log.Debug("Value " + m.DataPointObject.DatabaseName + ": in Alarmlist with lower level - firing");
                return FireStatus.Fire;
            }
            else if (th.AlarmType == entry.AlarmType)
            {
                log.Debug("Value " + m.DataPointObject.DatabaseName + ": in Alarmlist - level are the same - not firing");
                return FireStatus.Equal;
            }
            else
            {
                log.Debug("Value " + m.DataPointObject.DatabaseName + ": in Alarmlist - not firing");
                return FireStatus.NotFire;
            }
        }

        private async Task<FireStatus> CheckAgainst(AlarmThreshold th, Measurement m)
        {
            if (th != null)
            {
                AlarmListEntry entry = await MongoUoW.AlarmList.IsAlarmActiveForDataPoint(m.DataPointObject.DatabaseName);

                float fthreshold = Utilities.Converter.ConvertToFloat(th.Threashold);
                float fvalue = Utilities.Converter.ConvertToFloat(m.Value);
                Boolean bthreshold = Utilities.Converter.ConvertToBoolean(th.Threashold);
                Boolean bvalue = Utilities.Converter.ConvertToBoolean(m.Value);


                if (th.AlarmCheckType == AlarmCheckType.TopDown)
                {
                    if (fvalue < fthreshold)
                    {
                        log.Debug("Value " + m.DataPointObject.DatabaseName + ": is smaller than Threshold");

                        if (entry == null)
                        {
                            log.Debug("Value " + m.DataPointObject.DatabaseName + ": not in Alarmlist - firing");
                            return FireStatus.Fire; ;
                        }
                        else
                        {
                            return GetStatus(th, entry, m);
                        }
                    }
                }
                else if (th.AlarmCheckType == AlarmCheckType.BottomUp)
                {
                    if (fvalue >= fthreshold)
                    {
                        log.Debug("Value " + m.DataPointObject.DatabaseName + ": is greater than Threshold");

                        if (entry == null)
                        {
                            log.Debug("Value " + m.DataPointObject.DatabaseName + ": not in Alarmlist - firing");
                            return FireStatus.Fire;
                        }
                        else
                        {
                            return GetStatus(th, entry, m);
                        }
                    }
                }
                else if (th.AlarmCheckType == AlarmCheckType.NotEqual)
                {
                    if (bthreshold != bvalue)
                    {
                        log.Debug("Value " + m.DataPointObject.DatabaseName + ": is not Equal Threshold");

                        if (entry == null)
                        {
                            log.Debug("Value " + m.DataPointObject.DatabaseName + ": not in Alarmlist - firing");
                            return FireStatus.Fire; ;
                        }
                        else
                        {
                            return GetStatus(th, entry, m);
                        }
                    }
                }
                else if (th.AlarmCheckType == AlarmCheckType.Equal)
                {
                    if (bthreshold == bvalue)
                    {
                        log.Debug("Value " + m.DataPointObject.DatabaseName + ": is Equal Threshold");

                        if (entry == null)
                        {
                            log.Debug("Value " + m.DataPointObject.DatabaseName + ": not in Alarmlist - firing");
                            return FireStatus.Fire; ;
                        }
                        else
                        {
                            return GetStatus(th, entry, m);
                        }
                    }
                }
            }


            return FireStatus.NotFire;

        }


    }
    enum FireStatus
    {
        Fire,
        Equal,
        NotFire
    }

}
