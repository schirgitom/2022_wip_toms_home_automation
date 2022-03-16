using Context.DAL;
using Context.DAL.Data;
using Context.DAL.Visuals;
using Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Repository
{
    public interface IDataPointRepository :  IMongoRepository<DataPoint> 
    {

        Task AddVisualToDataPoint(DataPointVisual visual, DataPoint point);
        Task<DataPoint> FindByDataBaseName(String name);
    }
}
