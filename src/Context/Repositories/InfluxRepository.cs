using Context.DAL.Data;
using Context.DAL.InfluxDB;
using Context.Settings;
using Context.UnitOfWork;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Context.Repositories
{
    public class InfluxRepository
    {
        protected ILogger log = Logger.ContextLog<InfluxRepository>();
        protected InfluxDBContext InfluxDBContext = null;
        protected MongoDBUnitOfWork MongoDB;
        String bucket;
        String organisation;
        TimeSpan utcOffset;

        public InfluxRepository(MongoDBUnitOfWork mongo, InfluxDBContext context)
        {
            this.InfluxDBContext = context;
            this.MongoDB = MongoDB;
            bucket = context.Bucket;
            organisation = context.Organisation;

            utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
        }

        private PointData GeneratePoint(Sample measurement)
        {


            if (measurement.GetType() == typeof(BinarySample))
            {
                var point = PointData.Measurement(measurement.Tag)
                .Tag("measurement", measurement.Tag)
                .Field("value", measurement.AsBoolean())
                .Timestamp(measurement.TimeStamp.ToUniversalTime(), WritePrecision.S);
                return point;


            }
            else
            {
                var point = PointData.Measurement(measurement.Tag)
                .Tag("measurement", measurement.Tag)
                .Field("value", measurement.AsFloat())
                .Timestamp(measurement.TimeStamp.ToUniversalTime(), WritePrecision.S);
                return point;
            }

            return null;

        }

        public async Task InsertManyAsync(List<Sample> samples)
        {
            List<PointData> points = new List<PointData>();

            foreach(Sample sample in samples)
            {
                points.Add(GeneratePoint(sample));
            }

            await InfluxDBContext.WriteAPI.WritePointsAsync(points, InfluxDBContext.Bucket, InfluxDBContext.Organisation);

        }

        public async Task InsertOneAsync(Sample sample)
        {
            await InfluxDBContext.WriteAPI.WritePointAsync(GeneratePoint(sample), InfluxDBContext.Bucket, InfluxDBContext.Organisation);
        }

        public async Task<Sample> GetLast(DataPoint datapoint)
        {

            String flux = "from(bucket: \"" + bucket + "\")   |> range(start: -2h)   |> filter(fn: (r) => r[\"_measurement\"] == \"" + datapoint.DatabaseName + "\")  |> last()";

            var table = await InfluxDBContext.QueryAPI.QueryAsync(flux, organisation);

            List<Sample> samples = GetSamples(datapoint, table);

            if(samples != null && samples.Count > 0)
            {
                return samples[0];
            }

            return null;
        
        }

        public async Task<List<Sample>> GetInTimeRange(DataPoint datapoint, DateTime startdate, DateTime enddate)
        {
            long start = Utilities.Converter.ConvertDateToUnixTimeStamp(startdate);
            long end = Utilities.Converter.ConvertDateToUnixTimeStamp(enddate);

            String flux = "from(bucket: \"" + bucket + "\")   |> range(start: "  +start+", stop: " +end+ ")   |> filter(fn: (r) => r[\"_measurement\"] == \"" + datapoint.DatabaseName + "\") ";

            var table = await InfluxDBContext.QueryAPI.QueryAsync(flux, organisation);

            return GetSamples(datapoint, table);

        }

        private List<Sample> GetSamples(DataPoint dp, List<FluxTable> tables)
        {
            List<Sample> returnval = new List<Sample>();
            if (dp.DataType == DataType.Boolean)
            {
                foreach (var record in tables.SelectMany(table => table.Records))
                {
                    BinarySample smp = new BinarySample();
                    smp.TimeStamp = record.GetTime().Value.ToDateTimeUtc().ToLocalTime();
                    smp.Value = Boolean.Parse(record.GetValue().ToString());
                    smp.Tag = record.GetMeasurement();

                    returnval.Add(smp);
                }
            }
            else
            {
                foreach (var record in tables.SelectMany(table => table.Records))
                {
                    NumericSample smp = new NumericSample();
                    smp.TimeStamp = record.GetTime().Value.ToDateTimeUtc().ToLocalTime();
                    smp.Value = float.Parse(record.GetValue().ToString());
                    smp.Tag = record.GetMeasurement();

                    returnval.Add(smp);
                }
            }

            return returnval;
        }


    }
}
