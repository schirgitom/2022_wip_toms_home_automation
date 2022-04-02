using Context.DAL;
using Context.DAL.Alarming;
using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using Context.DAL.Visuals;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;

namespace UnitTests
{
    public class DataBaseSeedingTest : BaseUnitTests
    {
        [Test]
        public async Task TestSeeding()
        {
            await CreateVisuals();
            await CreateModbusDataSource();
            await CreateMQTTDataSource();
            await CreateUser();
            await CreateUser2();
        }

        public async Task CreateModbusDataSource()
        {
            ModbusDatasource ds = new ModbusDatasource();
            ds.Name = "Modbus";
            ds.Host = "127.0.0.1";
            ds.Port = 1502;
            ds.Active = true;
            ds.SlaveID = 1;

            DataSource returnval = await MongoUoW.DataSources.InsertOneAsync(ds);

            List<DataPoint> pts = new List<DataPoint>();

            DataPoint current = CreateFloatModbusDP("Chargepoint Current", "CurrentChargePoint", 1, 1);

            CreateAlarm(current, "Chargepoint Current reached Warning", AlarmCheckType.BottomUp, AlarmType.Warning, 13);
            CreateAlarm(current, "Chargepoint Current reached Aarning", AlarmCheckType.BottomUp, AlarmType.Alarm, 15);
            CreateAlarm(current, "Chargepoint Tripped", AlarmCheckType.BottomUp, AlarmType.Trip, 18);

            pts.Add(current);
            pts.Add(CreateFloatModbusDP("Chargepoint Voltage", "VoltageChargePoint", 3, 1));
            pts.Add(CreateBooleanModbusDP("Chargepoint Active", "ChargePointOn", 1));



            foreach (DataPoint pt in pts)
            {
                await MongoUoW.DataPoints.InsertOneAsync(pt);
                await MongoUoW.DataSources.AddDatapointToDataSoure(returnval, pt);
            }

            await AddVisualToPoint("Current", "ModbusCurrentChargePoint");
            await AddVisualToPoint("Voltage", "ModbusVoltageChargePoint");
            await AddVisualToPoint("OnOff", "ModbusChargePointOn");



            Assert.NotNull(returnval);
        }

        public async Task CreateUser()
        {
            User user = new User();
            user.UserName = "tom.schirgi@gmail.com";
            user.Role = Role.Admin;
            user.ValidTill = DateTime.MaxValue;
            user.Password = "12345";
            user.Firstname = "Thomas";
            user.Lastname = "Schirgi";
            User returnval = await MongoUoW.Users.InsertOneAsync(user);

            Assert.NotNull(returnval);
        }

        public async Task CreateUser2()
        {
            User user = new User();
            user.UserName = "tina.schirgi@gmail.com";
            user.Role = Role.User;
            user.ValidTill = DateTime.MaxValue;
            user.Password = "12345";
            user.Firstname = "Tina";
            user.Lastname = "Schirgi";

            User returnval = await MongoUoW.Users.InsertOneAsync(user);

            Assert.NotNull(returnval);
        }


        public async Task CreateMQTTDataSource()
        {
            MQTTDatasource ds = new MQTTDatasource();
            ds.Name = "MQTT";
            ds.Host = "127.0.0.1";
            ds.Port = 11883;
            ds.Active = true;


            var returnval = await MongoUoW.DataSources.InsertOneAsync(ds);

            List<DataPoint> pts = new List<DataPoint>();


            pts.Add(CreateFloatMQTTDP("Temperature Living Room", "TempLivingRoom"));
            pts.Add(CreateFloatMQTTDP("Temperature Entrance", "TempEntrance"));
            pts.Add(CreateFloatMQTTDP("Temperature Outside", "TempOutside"));
            pts.Add(CreateFloatMQTTDP("Power PV", "PowerPV"));
            pts.Add(CreateFloatMQTTDP("Temperature Water Flow", "TempFlow"));
            pts.Add(CreateFloatMQTTDP("Temperature Water Return", "TempReturn"));
            pts.Add(CreateBooleanMQTTDP("Garage", "Garage"));
            pts.Add(CreateBooleanMQTTDP("Climate", "Climate"));
            pts.Add(CreateBooleanMQTTDP("Climate Fail", "ClimateFail"));

            DataPoint glass = CreateBooleanMQTTDP("Glass Break Detector", "GlassBreakDetector");

            CreateAlarm(glass, "Glass Break Detected", AlarmCheckType.Equal, AlarmType.Trip, true);

            pts.Add(glass);

            DataPoint smoke = CreateBooleanMQTTDP("Smoke Detector", "SmokeDetector");

            CreateAlarm(smoke, "Smoke Detected", AlarmCheckType.Equal, AlarmType.Trip, true);

            pts.Add(smoke);



            foreach (DataPoint pt in pts)
            {
                await MongoUoW.DataPoints.InsertOneAsync(pt);
                await MongoUoW.DataSources.AddDatapointToDataSoure(returnval, pt);
            }

            await AddVisualToPoint("Power", "MQTTPowerPV");
            await AddVisualToPoint("Temperature", "MQTTTempLivingRoom");
            await AddVisualToPoint("Temperature", "MQTTTempEntrance");
            await AddVisualToPoint("Temperature", "MQTTTempOutside");
            await AddVisualToPoint("Temperature", "MQTTTempFlow");
            await AddVisualToPoint("Temperature", "MQTTTempReturn");
            await AddVisualToPoint("Garage", "MQTTGarage");
            await AddVisualToPoint("Climate", "MQTTClimate");
            await AddVisualToPoint("OK", "MQTTClimateFail");
            await AddVisualToPoint("Alarm", "MQTTGlassBreakDetector");
            await AddVisualToPoint("Alarm", "MQTTSmokeDetector");

            Assert.NotNull(returnval);
        }

        private DataPoint CreateAlarm(DataPoint pt, String text, AlarmCheckType type, AlarmType alarmtype, Object th)
        {
            AlarmThreshold th1 = new AlarmThreshold();
            th1.Message = text;
            th1.AlarmCheckType = type;
            th1.AlarmType = alarmtype;
            th1.Threashold = th;

            if (pt.AlarmThresholds.ContainsKey(th1.AlarmType.ToString()) == false)
            {
                pt.AlarmThresholds.Add(th1.AlarmType.ToString(), th1);
            }
            else
            {
                pt.AlarmThresholds[th1.AlarmType.ToString()] = th1;
            }


            return pt;
        }

        private async Task AddVisualToPoint(String visual, String point)
        {

            DataPointVisual visu = await MongoUoW.DataPointVisuals.FindByName(visual);
            DataPoint pt = await MongoUoW.DataPoints.FindByDataBaseName(point);

            if (visu != null && point != null)
            {
                await MongoUoW.DataPoints.AddVisualToDataPoint(visu, pt);
            }
           
        }


        private MQTTDataPoint CreateFloatMQTTDP(String name, string topic, int offset = 1)
        {
            MQTTDataPoint pt = new MQTTDataPoint();
            pt.Name = name;
            pt.DatabaseName = "MQTT" + topic;
            pt.TopicName = topic;
            pt.Offset = offset;
            pt.Description = name;
            pt.DataType = DataType.Float;



            return pt;

        }


        private MQTTDataPoint CreateBooleanMQTTDP(String name, string topic)
        {
            MQTTDataPoint pt = new MQTTDataPoint();
            pt.Name = name;
            pt.DatabaseName = "MQTT" + topic;
            pt.Description = name;
            pt.TopicName = topic;
            pt.DataType = DataType.Boolean;




            //pt.ValueMapping = mappings;

            return pt;

        }


        private ModbusDataPoint CreateFloatModbusDP(String name, String dbname, int register, int offset = 1)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.DatabaseName = "Modbus" + dbname;
            pt.Name = name;
            pt.RegisterCount = 2;
            pt.ReadingType = ReadingType.LowToHigh;
            pt.Register = register;
            pt.RegisterType = RegisterType.HoldingRegister;
            pt.Offset = offset;
            pt.DataType = DataType.Float;

            return pt;

        }

        private ModbusDataPoint CreateBooleanModbusDP(String name, String dbname, int register)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.DatabaseName = "Modbus" + dbname;
            pt.Name = name;
            pt.RegisterCount = 1;
            pt.Register = register;
            pt.RegisterType = RegisterType.Coil;
            pt.DataType = DataType.Boolean;

            return pt;

        }


        #region Visuals


        private async Task CreateVisuals()
        {
            await CreateCurrentVisual();
            await CreateVoltageVisual();
            await CreatePowerVisual();
            await CreateTemperatureVisual();
            await CreateGaragaeVisual();
            await CreateClimateVisual();
            await CreateOKVisual();
            await CreateAlarmVisual();
            await CreateOnVisual();
        }

        private async Task CreateCurrentVisual()
        {
            NumericDataPointVisuals visu = new NumericDataPointVisuals();
            visu.Name = "Current";
            visu.Description = "Current";
            visu.Icon = "Electrcity";
            visu.MinValue = 0;
            visu.MaxValue = 20;
            visu.Unit = "A";

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreateVoltageVisual()
        {
            NumericDataPointVisuals visu = new NumericDataPointVisuals();
            visu.Name = "Voltage";
            visu.Description = "Voltage";
            visu.Icon = "Volt";
            visu.MinValue = 0;
            visu.MaxValue = 400;
            visu.Unit = "V";

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreatePowerVisual()
        {
            NumericDataPointVisuals visu = new NumericDataPointVisuals();
            visu.Name = "Power";
            visu.Description = "Power";
            visu.Icon = "Power";
            visu.MinValue = 0;
            visu.MaxValue = 400;
            visu.Unit = "kW";

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }


        private async Task CreateTemperatureVisual()
        {
            NumericDataPointVisuals visu = new NumericDataPointVisuals();
            visu.Name = "Temperature";
            visu.Description = "Temperature";
            visu.Icon = "Temperature";
            visu.MinValue = -20;
            visu.MaxValue = 50;
            visu.Unit = "°C";


            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }


        private async Task CreateGaragaeVisual()
        {
            BinaryDataPointVisuals visu = new BinaryDataPointVisuals();
            visu.Name = "Garage";
            visu.Description = "Garage";
            visu.Icon = "Car";

            List<BinaryValueMapping> mappings = new List<BinaryValueMapping>();
            BinaryValueMapping mping = new BinaryValueMapping();
            mping.Value = true;
            mping.Text = "Open";

            BinaryValueMapping mping2 = new BinaryValueMapping();
            mping2.Value = false;
            mping2.Text = "Closed";

            mappings.Add(mping);
            mappings.Add(mping2);

            visu.ValueMapping = mappings;

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreateClimateVisual()
        {
            BinaryDataPointVisuals visu = new BinaryDataPointVisuals();
            visu.Name = "Climate";
            visu.Description = "Climate";
            visu.Icon = "Climate";

            List<BinaryValueMapping> mappings = new List<BinaryValueMapping>();
            BinaryValueMapping mping = new BinaryValueMapping();
            mping.Value = true;
            mping.Text = "On";

            BinaryValueMapping mping2 = new BinaryValueMapping();
            mping2.Value = false;
            mping2.Text = "Off";

            mappings.Add(mping);
            mappings.Add(mping2);

            visu.ValueMapping = mappings;

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreateOKVisual()
        {
            BinaryDataPointVisuals visu = new BinaryDataPointVisuals();
            visu.Name = "OK";
            visu.Description = "OK";
            visu.Icon = "Check";

            List<BinaryValueMapping> mappings = new List<BinaryValueMapping>();
            BinaryValueMapping mping = new BinaryValueMapping();
            mping.Value = true;
            mping.Text = "OK";

            BinaryValueMapping mping2 = new BinaryValueMapping();
            mping2.Value = false;
            mping2.Text = "Not OK";

            mappings.Add(mping);
            mappings.Add(mping2);

            visu.ValueMapping = mappings;

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreateAlarmVisual()
        {
            BinaryDataPointVisuals visu = new BinaryDataPointVisuals();
            visu.Name = "Alarm";
            visu.Description = "Alarm";
            visu.Icon = "Alarm";

            List<BinaryValueMapping> mappings = new List<BinaryValueMapping>();
            BinaryValueMapping mping = new BinaryValueMapping();
            mping.Value = true;
            mping.Text = "OK";

            BinaryValueMapping mping2 = new BinaryValueMapping();
            mping2.Value = false;
            mping2.Text = "Alarm";

            mappings.Add(mping);
            mappings.Add(mping2);

            visu.ValueMapping = mappings;

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }

        private async Task CreateOnVisual()
        {
            BinaryDataPointVisuals visu = new BinaryDataPointVisuals();
            visu.Name = "OnOff";
            visu.Description = "OnOff";
            visu.Icon = "OnOff";

            List<BinaryValueMapping> mappings = new List<BinaryValueMapping>();
            BinaryValueMapping mping = new BinaryValueMapping();
            mping.Value = true;
            mping.Text = "On";

            BinaryValueMapping mping2 = new BinaryValueMapping();
            mping2.Value = false;
            mping2.Text = "Off";

            mappings.Add(mping);
            mappings.Add(mping2);

            visu.ValueMapping = mappings;

            await MongoUoW.DataPointVisuals.InsertOneAsync(visu);


        }


        #endregion



        public async Task AddVisualTest()
        {
            DataPointVisual visu = await MongoUoW.DataPointVisuals.FindByName("Current");


            List<MQTTDatasource> returnval = await MongoUoW.DataSources.GetMQTTDataSources();

            foreach (MQTTDatasource mqttDataSource in returnval)
            {
                foreach (DataPoint dp in mqttDataSource.DataPoints)
                {
                    await MongoUoW.DataPoints.AddVisualToDataPoint(visu, dp);
                }
            }


        }

    }
}