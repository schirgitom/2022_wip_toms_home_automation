using Context.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories.Concrete
{
    public interface IDatasourceRepository : IMongoRepository<DataSource>
    {
        Task AddDatapointToDataSoure(DataSource dataSource, DataPoint point);
    }
}
