using Context;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace UnitTest
{
    public class BaseUnitTests
    {
        protected ILogger log = Logger.ContextLog<BaseUnitTests>();
    

        [OneTimeSetUp]
        public async Task Setup()
        {
            Logger.InitLogger();
     
        }

        [Test]
        public void MyFirstLog()
        {
            log.Information("My first try");
            Assert.IsTrue(true);
        }



    }
}
