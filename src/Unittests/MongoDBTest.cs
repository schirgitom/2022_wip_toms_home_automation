using Context.DAL.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTest;

namespace Unittests
{
    
    public class MongoDBTest : BaseUnitTests
    {
        [Test]
        public async Task TestConnect()
        {
            Assert.IsTrue(MongoContext.IsConnected);
        }

        [Test]
        public async Task CreateDataPoint()
        {
            DataPoint pt = new DataPoint();
            pt.DataType = DataType.Float;
            pt.Description = "Erster Test";
            pt.Name = "Erster Test";
            
            await MongoUoW.DataPoints.InsertOneAsync(pt);
            Assert.NotNull(pt);
            Assert.NotNull(pt.ID);
        }
    }
}
