using Context.DAL.Data;
using Context.DAL.InfluxDB;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;

namespace Unittests
{
    public class InfluxDBTests : BaseUnitTests
    {
        private List<Sample> GenerateDummyData()
        {
            List<Sample> data = new List<Sample>();

            DateTime now = DateTime.Now;
            DateTime last = DateTime.Now.AddDays(-2);

            TimeSpan span = now - last;
            Random random = new Random();

            long minutes = (long)span.TotalMinutes;

            for (int i = 0; i < minutes; i++)
            {
                NumericSample sample = new NumericSample();
                sample.Tag = "AquariumTemp";
                sample.Value = (float)random.NextDouble() * 33;
                sample.TimeStamp = DateTime.Now.AddMinutes(i * -1);

                data.Add(sample);
            }

            return data;
        }

        [Test]
        public async Task CreateEntries()
        {
            List<Sample> samples = GenerateDummyData();
            await InfluxDBUnitOfWork.Repository.InsertManyAsync(samples);

            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetLastValue()
        {
            DataPoint dp = new DataPoint();
            dp.DatabaseName = "AquariumTemp";
            dp.Name = "AquariumTemp";
            dp.DataType = DataType.Float;

            Sample sample = await InfluxDBUnitOfWork.Repository.GetLast(dp);

            Assert.NotNull(sample);
            Assert.Greater(sample.AsFloat(), 0);
        }

        [Test]
        public async Task GetInRange()
        {
            DataPoint dp = new DataPoint();
            dp.DatabaseName = "AquariumTemp";
            dp.Name = "AquariumTemp";
            dp.DataType = DataType.Float;

            List<Sample> samples = await InfluxDBUnitOfWork.Repository.GetInTimeRange(dp, DateTime.Now.AddHours(-2), DateTime.Now);
            Assert.NotNull(samples);
            Assert.Greater(samples.Count(), 1);

        }
    }
}
