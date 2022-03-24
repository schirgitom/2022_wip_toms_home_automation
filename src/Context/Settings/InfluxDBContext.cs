using InfluxDB.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Settings
{
    public class InfluxDBContext
    {
        ILogger log = Utilities.Logger.ContextLog<InfluxDBContext>();

        public InfluxDBClient DataBase { get; private set; }

        public String Bucket { get; private set; }

        public String Organisation { get; private set; }

        public QueryApi QueryAPI { get; private set; }

        public WriteApiAsync WriteAPI { get; private set; }

        public Boolean IsConnected
        {
            get
            {
                return DataBase != null;
            }
        }

        public InfluxDBContext(InfluxDBSettings settings)
        {


            String url = "http://" + settings.Server + ":" + settings.Port;

            log.Debug("Connecting to Influx Database: " + url);

            DataBase = InfluxDBClientFactory.Create(url, settings.Token);

            Bucket = settings.Bucket;
            Organisation = settings.Organization;


            if (DataBase != null)
            {
                log.Information("Successfully connected to Influx DB " + settings.Server + ":" + settings.Port);

                QueryAPI = DataBase.GetQueryApi();

                WriteAPI = DataBase.GetWriteApiAsync();
            }
            else
            {
                log.Fatal("Could not connect to Influx DB " + settings.Server + ":" + settings.Port);
            }

        }
    }
}
