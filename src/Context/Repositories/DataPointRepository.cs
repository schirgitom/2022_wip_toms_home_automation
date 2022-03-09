using Context.DAL.Data;
using Context.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repositories
{
    public class DataPointRepository : MongoRepository<DataPoint>, IDataPointRepository
    {
        public DataPointRepository(MongoDBContext Context) : base(Context)
        {
        }
    }
}
