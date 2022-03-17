using Context.DAL.Data;
using Context.DAL.Data.Sources;
using Context.Settings;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories.Concrete
{
    public class DatasourceRepository : MongoRepository<DataSource>, IDatasourceRepository
    {
        public DatasourceRepository(MongoDBContext Context) : base(Context)
        {
        }

        public async Task AddDatapointToDataSoure(DataSource dataSource, DataPoint point)
        {
            point.DataSource = dataSource.ToReference();
            point.SaveAsync();

            await dataSource.Internal_DataPoints.AddAsync(point);
        }


        public async Task<DataPoint> GetDatapoint(string name)
        {
            List<DataSource> datasources = await GetDataSources();

            if (datasources != null)
            {
                DataPoint dp = (from subitem in datasources from item in subitem.DataPoints where item.DatabaseName.Equals(name) select item).FirstOrDefault();

                if (dp != null)
                {
                    return dp;
                }
            }

            return null;

        }

        public async Task<List<DataPoint>> GetDatapoints()
        {
            List<DataSource> datasources = await GetDataSources();

            List<DataPoint> datapoints = new List<DataPoint>();
            if (datasources != null)
            {
                datapoints = (from subitem in datasources from item in subitem.DataPoints select item).ToList();

                if (datapoints != null)
                {
                    return datapoints;
                }
            }

            return null;

        }

        public async Task<DataSource> GetDataSource(string name)
        {
            return base.FindOne(x => x.Name == name);
        }

        public async Task<List<DataSource>> GetDataSources()
        {
            return base.FilterBy(x => x.Active == true).ToList();
        }

        public async Task<List<ModbusDatasource>> GetModbusDataSources()
        {


            List<DataSource> sources2 = await DB.Find<DataSource>()
                    .Match(_ => true)
                    .ExecuteAsync();
            List<ModbusDatasource> returnval = new List<ModbusDatasource>();

            foreach (DataSource src in sources2)
            {
                if (src.GetType() == typeof(ModbusDatasource))
                {
                    //src.DataPoints.Lo
                    returnval.Add((ModbusDatasource)src);
                }
            }


            return returnval;


        }

        public async Task<List<MQTTDatasource>> GetMQTTDataSources()
        {

            List<DataSource> sources2 = await DB.Find<DataSource>()
                    .Match(_ => true)
                    .ExecuteAsync();
            List<MQTTDatasource> returnval = new List<MQTTDatasource>();

            foreach (DataSource src in sources2)
            {
                if (src.GetType() == typeof(MQTTDatasource))
                {
                    //src.DataPoints.Lo
                    returnval.Add((MQTTDatasource)src);
                }
            }


            return returnval;
        }
    }
}
