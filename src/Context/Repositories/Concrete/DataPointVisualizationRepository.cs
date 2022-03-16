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
    public class DataPointVisualizationRepository : MongoRepository<DataPointVisual>,  IDataPointVisualizationRepository
    {
        public DataPointVisualizationRepository(MongoDBContext Context) : base(Context)
        {

        }

        public async Task<DataPointVisual> FindByName(string name)
        {
            return await FindOneAsync(x => x.Name == name);
        }
    }
}
