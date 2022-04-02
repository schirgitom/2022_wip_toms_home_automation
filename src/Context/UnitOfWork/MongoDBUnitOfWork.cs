using Context.DAL.Data;
using Context.Repositories;
using Context.Repositories.Concrete;
using Context.Repository;
using Context.Settings;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.UnitOfWork
{
    public class MongoDBUnitOfWork
    {
        public MongoDBContext Context { get; private set; } = null;
        public MongoDBUnitOfWork()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Utilities.Constants.CurrentFolder).AddJsonFile("appsettings.json");

            MongoDBSettings settings = builder.Build().GetSection("MongoDbSettings").Get<MongoDBSettings>();
            MongoDBContext context = new MongoDBContext(settings);
            Context = context;
        }

        public IDataPointRepository DataPoints
        {
            get
            {
                return new DataPointRepository(Context);
            }
        }
        public IUserRepository Users
        {
            get
            {
                return new UserRepository(Context);
            }
        }


        public IDataPointVisualizationRepository DataPointVisuals
        {
            get
            {
                return new DataPointVisualizationRepository(Context);
            }
        }

        public IDatasourceRepository DataSources
        {
            get
            {
                return new DatasourceRepository(Context);
            }
        }

        public IAlarmListRepository AlarmList
        {
            get
            {
                return new AlarmListRepository(Context);
            }
        }

    }
}
