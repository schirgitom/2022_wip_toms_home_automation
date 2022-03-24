using Context;
using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using Context.Drivers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;

namespace UnitTests
{
    public class ModbusTest : BaseUnitTests
    {
        
        public ModbusTest()
        {
      
        }

        DataPoint NumericPoint;
        DataPoint BooleanPoint;

        ModbusDatasource Source;

        [OneTimeSetUp]
        public async Task Init()
        {


            ModbusDatasource dbsrc = await MongoUoW.DataSources.FindOneAsync(y => y.Name == "Modbus") as ModbusDatasource;

            if (dbsrc == null)
            {
                ModbusDatasource ds = new ModbusDatasource();
                ds.Name = "Modbus";
                ds.Host = "127.0.0.1";
                ds.Port = 1502;
                ds.Active = true;
                ds.SlaveID = 1;

                DataSource returnval = await MongoUoW.DataSources.InsertOneAsync(ds);

                List<DataPoint> pts = new List<DataPoint>();

                pts.Add(CreateFloatModbusDP("Chargepoint Current", 1, 1));
                pts.Add(CreateFloatModbusDP("Chargepoint Voltage",  3, 1));
                pts.Add(CreateBooleanModbusDP("Chargepoint Active", 1));
            



                foreach (DataPoint pt in pts)
                {
                    await MongoUoW.DataPoints.InsertOneAsync(pt);
                    await MongoUoW.DataSources.AddDatapointToDataSoure(returnval, pt);
                }

                Source = ds;
            }
            else
            {
                Source = dbsrc;
            }


        }


        [Test]
        public async Task Read()
        {

            ModbusDriver driver = new ModbusDriver(Source);

            await driver.Create();
            await driver.Connect();

            Assert.IsTrue(driver.IsConnected);

            await driver.Read();

            Dictionary<String, List<Measurement>> mn = await driver.NumericData();

            Assert.NotNull(mn);
            Assert.Greater(mn.Count, 1);

        }


        private ModbusDataPoint CreateFloatModbusDP(String name, int register, int offset = 1)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.DatabaseName = "Modbus" + name;
            pt.Name = name;
            pt.RegisterCount = 2;
            pt.ReadingType = ReadingType.LowToHigh;
            pt.Register = register;
            pt.RegisterType = RegisterType.HoldingRegister;
            pt.Offset = offset;
            pt.DataType = DataType.Float;

            return pt;

        }

        private ModbusDataPoint CreateBooleanModbusDP(String name, int register)
        {
            ModbusDataPoint pt = new ModbusDataPoint();
            pt.DatabaseName = "Modbus" + name;
            pt.Name = name;
            pt.RegisterCount = 1;
            pt.Register = register;
            pt.RegisterType = RegisterType.Coil;
            pt.DataType = DataType.Boolean;

            return pt;

        }


    }
}
