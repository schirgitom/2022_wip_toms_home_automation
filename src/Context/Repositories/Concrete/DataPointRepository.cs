using Context.DAL;
using Context.DAL.Data;
using Context.DAL.Visuals;
using Context.Repositories;
using Context.Settings;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repository
{
    public class DataPointRepository : MongoRepository<DataPoint>,  IDataPointRepository
    {
        public DataPointRepository(MongoDBContext Context) : base(Context)
        {

        }



        public async Task AddVisualToDataPoint(DataPointVisual visual, DataPoint point)
        {
            point.Internal_Visual = visual.ToReference();
            await point.SaveAsync();
        }

        public async Task<DataPoint> FindByDataBaseName(string name)
        {
                return await FindOneAsync(x => x.DatabaseName == name);
        }
    }
}
