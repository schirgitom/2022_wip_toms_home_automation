using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using Context.Drivers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitTest;

namespace Unittests
{
    public class MQTTTest : BaseUnitTests
    {
        MQTTDriver driver = null;


        [TearDown]
        public async Task TearDown()
        {
            if (driver != null)
            {
                driver.Disconnect();
            }
        }


        [Test]
        public async Task TestConnection()
        {

            MQTTDatasource dbsrc = await MongoUoW.DataSources.FindOneAsync(y => y.Name == "MQTT") as MQTTDatasource;

            if (dbsrc == null)
            {
                
                MQTTDatasource ds = new MQTTDatasource();
                ds.Name = "MQTT";
                ds.Host = "127.0.0.1";
                ds.Port = 11883;
                ds.Active = true;


                var returnval = await MongoUoW.DataSources.InsertOneAsync(ds);

                List<DataPoint> pts = new List<DataPoint>();


                pts.Add(CreateFloatMQTTDP("Wohnzimmer Temperatur", "TempLivingRoom"));

                pts.Add(CreateBooleanMQTTDP("Garage", "Garage"));

                foreach (DataPoint pt in pts)
                {
                    await MongoUoW.DataPoints.InsertOneAsync(pt);
                    await MongoUoW.DataSources.AddDatapointToDataSoure(returnval, pt);
                }

            }

            MQTTDatasource src = await MongoUoW.DataSources.FindOneAsync(y => y.Name == "MQTT") as MQTTDatasource;



            driver = new MQTTDriver(src);
            await driver.Create();
            driver.Connect();

            Thread.Sleep(5000);

            Assert.IsTrue(driver.IsConnected);
        }


        [Test]
        public async Task ReceiveValue()
        {

            MQTTDatasource dbsrc = await MongoUoW.DataSources.FindOneAsync(y => y.Name == "MQTT") as MQTTDatasource;

            if (dbsrc == null)
            {

                MQTTDatasource ds = new MQTTDatasource();
                ds.Name = "MQTT";
                ds.Host = "127.0.0.1";
                ds.Port = 11883;
                ds.Active = true;


                var returnval = await MongoUoW.DataSources.InsertOneAsync(ds);

                List<DataPoint> pts = new List<DataPoint>();


                pts.Add(CreateFloatMQTTDP("Wohnzimmer Temperatur", "TempLivingRoom"));
                pts.Add(CreateBooleanMQTTDP("Garage", "Garage"));

                foreach (DataPoint pt in pts)
                {
                    await MongoUoW.DataPoints.InsertOneAsync(pt);
                    await MongoUoW.DataSources.AddDatapointToDataSoure(returnval, pt);
                }

            }

            List<MQTTDatasource> src = await MongoUoW.DataSources.GetMQTTDataSources();

            driver = new MQTTDriver(src[0]);
            await driver.Create();
            await driver.Connect();

            await Task.Delay(15000);

            // Assert.IsTrue(driver.IsConnected);

            Dictionary<String, List<Measurement>> meas = await driver.NumericData();

            Assert.IsNotNull(meas);
            Assert.IsTrue(meas.ContainsKey("MQTTTempLivingRoom"));
            Assert.Greater(meas["MQTTTempLivingRoom"].Count, 0);

        }


        protected MQTTDataPoint CreateFloatMQTTDP(String name, string topic, int offset = 1)
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

        protected MQTTDataPoint CreateBooleanMQTTDP(String name, string topic)
        {
            MQTTDataPoint pt = new MQTTDataPoint();
            pt.Name = name;
            pt.DatabaseName = "MQTT" + topic;
            pt.Description = name;
            pt.TopicName = topic;
            pt.DataType = DataType.Boolean;


            return pt;

        }

    }
}
