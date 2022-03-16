using Context.DAL.Data;
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
    public class DataSourceUnitTest : BaseUnitTests
    {
        [Test]
        public async Task CreateModbusSource()
        {
            ModbusDatasource datasource = new ModbusDatasource();
            datasource.Active = true;
            datasource.SlaveID = 1;
            datasource.Host = "127.0.0.1";
            datasource.Port = 1502;
            datasource.Name = "ModbusTest";
            
            await MongoUoW.DataSources.InsertOrUpdateOneAsync(datasource);

            DataSource mdbus = await MongoUoW.DataSources.FindOneAsync(x => x.Name == "ModbusTest");

            Assert.NotNull(mdbus);

        }

        [Test]
        public async Task CreateMQTTSource()
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
    }
}
