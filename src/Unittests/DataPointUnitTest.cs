using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;

namespace Unittests
{
    public class DataPointUnitTest : BaseUnitTests
    {
        [Test]
        public async Task CreateModbusDataPoint()
        {
            MQTTDatasource datasource = new MQTTDatasource();
            datasource.Active = true;
            datasource.Host = "127.0.0.1";
            datasource.Port = 11883;
            datasource.Name = "MQTTTest";

            await MongoUoW.DataSources.InsertOrUpdateOneAsync(datasource);


            MQTTDataPoint dataPoint = new MQTTDataPoint();
            dataPoint.Name = "Wohnzimmer Temp";
            dataPoint.TopicName = "TempLivingRoom";
            dataPoint.DatabaseName = "MQTTTempLivingRoom";
            dataPoint.DataType = DataType.Float;
            dataPoint.Offset = 1;

            //await 

            await MongoUoW.DataPoints.InsertOrUpdateOneAsync(dataPoint);
            await MongoUoW.DataSources.AddDatapointToDataSoure(datasource, dataPoint);

            MQTTDatasource mqttfromdb = await MongoUoW.DataSources.FindByIdAsync(datasource.ID) as MQTTDatasource;

            Assert.NotNull(mqttfromdb);
            Assert.NotNull(mqttfromdb.DataPoints);
            Assert.Greater(mqttfromdb.DataPoints.Count, 0);

        }

        [Test]
        public async Task CreateMQTTDataPoint()
        {
            MQTTDatasource datasource = new MQTTDatasource();
            datasource.Active = true;
            datasource.Host = "127.0.0.1";
            datasource.Port = 11883;
            datasource.Name = "MQTTTest";

            await MongoUoW.DataSources.InsertOrUpdateOneAsync(datasource);

            MQTTDatasource mdbus = await MongoUoW.DataSources.FindOneAsync(x => x.Name == "MQTTTest") as MQTTDatasource;

            Assert.NotNull(mdbus);

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
    }
}
