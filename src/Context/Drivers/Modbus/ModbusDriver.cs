using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using Modbus.Device;
using Modbus.Utility;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Context.Drivers
{
    public class ModbusDriver : Driver
    {
        TcpClient Client;
        ModbusIpMaster Master;



        private ModbusDatasource Source;
        String FinalUrl;
        List<ModbusDataPoint> i_DataPoints = new List<ModbusDataPoint>();

        public ModbusDriver(ModbusDatasource src) : base(src.Name)
        {
            this.Source = src;
        }

        public async override Task Create()
        {
            try
            {
                FinalUrl = Source.Host + ":" + Source.Port;

                Client = new TcpClient(Source.Host, Source.Port);
                Master = ModbusIpMaster.CreateIp(Client);

            }
            catch (Exception ex)
            {
                log.Fatal("Could not create Master ", ex);
            }



        }

        public async override Task Connect()
        {

            IsConnected = Client.Connected;

            i_DataPoints.Clear();

            foreach (DataPoint dpi in this.Source.DataPoints)
            {

                if (dpi.GetType() == typeof(ModbusDataPoint))
                {
                    ModbusDataPoint dp = (ModbusDataPoint)dpi;
                    i_DataPoints.Add(dp);
                    AddDataPoint(dp.DatabaseName, dp);
                }

            }

        }





        public async override Task Disconnect()
        {
            try
            {

                log.Information("Stopping Client");
                if (Client != null)
                {
                    Client.Dispose();
                }
            }
            catch (Exception e)
            {
                log.Warning("Stopping failed " + e.ToString());
            }
        }

        public async override Task Read()
        {

            if (IsConnected)
            {
                foreach (ModbusDataPoint pt in i_DataPoints)
                {

                    if (pt.RegisterCount > 0 && pt.Register > 0)
                    {
                        Measurement mn = await GetModbusVal(pt);

                        if (mn != null)
                        {

                            if (pt.DataType == DataType.Float)
                            {
                                if (pt.Offset > 0)
                                {
                                    mn.Value = (float)(Utilities.Converter.ConvertToDouble(mn.Value) / pt.Offset);
                                }

                                AddNumericMeasurement(pt.DatabaseName, mn);
                            }
                            else
                            {
                                AddBinaryMeasurement(pt.DatabaseName, mn);
                            }

                            log.Debug($"Got {mn.Value} from {pt.DatabaseName} ");
                        }
                    }
                    else
                    {
                        log.Error("Cannot Read " + pt.DatabaseName + " Register or Offset 0");
                    }
                }
            }
        }


        private Measurement DecodeNumeric(ushort[] register, Measurement sample, ModbusDataPoint pt)
        {
            if (pt.DataType == DataType.Float)
            {

                if (register.Count() > 1)
                {
                    if (register.Count() == 2)
                    {
                        ushort register0 = register[0];
                        ushort register1 = register[1];

                        if (pt.ReadingType == ReadingType.HighToLow)
                        {
                            register0 = register[1];
                            register1 = register[0];
                        }

                        sample.Value = ModbusUtility.GetSingle(register0, register1);


                        log.Verbose($"Reading {this.Name} Quantity 2 - Address {pt.Register}/{pt.RegisterCount} returned {sample.Value}");
                    }
                    else
                    {
                        sample.Value = 0;
                        log.Verbose($"Reading {this.Name} Quantity 2 - Address {pt.Register}/{pt.RegisterCount} Not found");
                    }
                }
                else if (register.Count() == 1)
                {
                    ushort register0 = register[0];
                    sample.Value = Utilities.Converter.ConvertToFloat(register0);
                }
                else
                {
                    sample.Value = 0;
                }
            }

            return sample;

        }

        private async Task<Measurement> GetModbusVal(ModbusDataPoint pt)
        {
            Measurement sample = new Measurement();
            sample.DataPointObject = pt;
            sample.Time = DateTime.Now;

            ushort start = Convert.ToUInt16(pt.Register);
            ushort offset = Convert.ToUInt16(pt.RegisterCount);
            try
            {
                if (pt.RegisterType == RegisterType.Coil)
                {

                    bool[] returnv = await Master.ReadCoilsAsync(start, 1);

                    if (returnv != null && returnv.Length > 0)
                    {
                        sample.Value = Utilities.Converter.ConvertToBoolean(returnv[0]);
                    }
                    else
                    {
                        log.Warning("Modbus Point " + pt.Name + " could not be found: Register : " + start + " Offset: " + offset);
                        sample.Value = false;
                    }


                }
                else if (pt.RegisterType == RegisterType.InputStatus)
                {

                    ushort[] returnv = await Master.ReadInputRegistersAsync(start, offset);

                    if (returnv != null && returnv.Length > 0)
                    {
                        sample.Value = Utilities.Converter.ConvertToBoolean(returnv[0]);
                    }
                    else
                    {
                        log.Warning("Modbus Point " + pt.Name + " could not be found: Register : " + start + " Offset: " + offset);
                        sample.Value = false;
                    }

                }
                else if (pt.RegisterType == RegisterType.InputRegister)
                {


                    ushort[] register = await Master.ReadInputRegistersAsync(start, offset);

                    sample = DecodeNumeric(register, sample, pt);


                }
                else if (pt.RegisterType == RegisterType.HoldingRegister)
                {


                    ushort[] register = await Master.ReadHoldingRegistersAsync(start, offset);

                    sample = DecodeNumeric(register, sample, pt);


                }


            }
            catch (Exception e)
            {
                log.Error($"Could not read register {pt.DatabaseName} and quantity {start}", e);
                sample = null;
            }
            return sample;
        }




    }
}
