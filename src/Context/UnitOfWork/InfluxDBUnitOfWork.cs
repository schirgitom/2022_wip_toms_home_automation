using Context.DAL.InfluxDB;
using Context.Drivers;
using Context.Repositories;
using Context.Settings;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Context.UnitOfWork
{
    public class InfluxDBUnitOfWork
    {

        protected ILogger log = Logger.ContextLog<InfluxDBUnitOfWork>();

        public InfluxDBContext Context { get; private set; } = null;
        public MongoDBUnitOfWork MongoUoW { get; set; }

        private InfluxRepository _Repository = null;

        public InfluxDBUnitOfWork(MongoDBUnitOfWork mongo)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Utilities.Constants.CurrentFolder).AddJsonFile("appsettings.json");

            InfluxDBSettings settings = builder.Build().GetSection("InfluxDBSettings").Get<InfluxDBSettings>();
            InfluxDBContext context = new InfluxDBContext(settings);
            Context = context;
            MongoUoW = mongo;

            _Repository = new InfluxRepository(mongo, Context);
        }

        public InfluxRepository Repository
        {
            get { return _Repository; }
        }

        public List<Sample> ConvertToNumericList(Dictionary<String, List<Measurement>> input)
        {
            List<Sample> samples = new List<Sample>();

            foreach(KeyValuePair<String, List<Measurement>> pair in input)
            {
                foreach(Measurement m in pair.Value)
                {
                    NumericSample sample = new NumericSample();
                    sample.Tag = pair.Key;
                    sample.TimeStamp = m.Time;
                    sample.Value = m.Value;

                    samples.Add(sample);
                }
            }

            return samples;
        }

        public List<Sample> ConvertToBinaryList(Dictionary<String, List<Measurement>> input)
        {
            List<Sample> samples = new List<Sample>();

            foreach (KeyValuePair<String, List<Measurement>> pair in input)
            {
                foreach (Measurement m in pair.Value)
                {
                    BinarySample sample = new BinarySample();
                    sample.Tag = pair.Key;
                    sample.TimeStamp = m.Time;
                    sample.Value = m.Value;

                    samples.Add(sample);
                }
            }

            return samples;
        }
    }
}
