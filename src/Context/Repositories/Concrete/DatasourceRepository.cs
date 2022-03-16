using Context.DAL.Data;
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
    }
}
