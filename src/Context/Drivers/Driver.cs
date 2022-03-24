using Context.DAL.Data;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Context.Drivers
{
    public abstract class Driver
    {
        protected ILogger log = Logger.ContextLog<Driver>();

        protected ConcurrentDictionary<String, ConcurrentBag<Measurement>> NumericMeasurements = new ConcurrentDictionary<String, ConcurrentBag<Measurement>>();
        protected ConcurrentDictionary<String, ConcurrentBag<Measurement>> BinaryMeasurements = new ConcurrentDictionary<String, ConcurrentBag<Measurement>>();
        protected Dictionary<String, DataPoint> DataPoints = new Dictionary<String, DataPoint>();
        public String Name { get; set; }

        public Boolean IsConnected { get; protected set; }

        protected Driver(string namen)
        {
            Name = namen;
        }

        public abstract Task Connect();
        public abstract Task Disconnect();
        public abstract Task Create();
        public abstract Task Read();

        public void AddNumericMeasurement(String datapoint, Measurement meas)
        {
            if(!NumericMeasurements.ContainsKey(datapoint))
            {
                NumericMeasurements.TryAdd(datapoint, new ConcurrentBag<Measurement>());
            }

            NumericMeasurements[datapoint].Add(meas);
        }

        public void AddBinaryMeasurement(String datapoint, Measurement meas)
        {
            if (!BinaryMeasurements.ContainsKey(datapoint))
            {
                BinaryMeasurements.TryAdd(datapoint, new ConcurrentBag<Measurement>());
            }

            BinaryMeasurements[datapoint].Add(meas);
        }

        public async Task Clear()
        {
            NumericMeasurements.Clear();
            BinaryMeasurements.Clear();
        }

        public void AddDataPoint(String name, DataPoint pt)
        {
            if(!DataPoints.ContainsKey(name))
            {
                DataPoints.Add(name, pt);
            }
        }

        public DataPoint GetDataPoint(String namen)
        {
            if(DataPoints.ContainsKey(namen))
            {
                return DataPoints[namen];
            }

            return null;
        }

        public async Task<Dictionary<String, List<Measurement>>> NumericData()
        {
            return NumericMeasurements.ToDictionary(x => x.Key, y => y.Value.ToList());
        }

        public async Task<Dictionary<String, List<Measurement>>> BinaryData()
        {
            return BinaryMeasurements.ToDictionary(x => x.Key, y => y.Value.ToList());
        }

    }
}
