using Context.DAL.Data;
using Context.DAL.Data.Sources;
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



        Task<List<ModbusDatasource>> GetModbusDataSources();
        Task<List<MQTTDatasource>> GetMQTTDataSources();
        Task<List<DataSource>> GetDataSources();
        Task<DataSource> GetDataSource(String name);
        Task<DataPoint> GetDatapoint(String name);
        Task<List<DataPoint>> GetDatapoints();
    }
}
