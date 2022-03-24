﻿using Context;
using Context.Settings;
using Context.UnitOfWork;
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
        protected MongoDBUnitOfWork MongoUoW = null;
        protected MongoDBContext MongoContext = null;

        protected InfluxDBContext InfluxDBContext = null;
        protected InfluxDBUnitOfWork InfluxDBUnitOfWork = null;

            [OneTimeSetUp]
        public async Task Setup()
        {
            Logger.InitLogger();

            MongoUoW = new MongoDBUnitOfWork();
            MongoContext = MongoUoW.Context;

            InfluxDBUnitOfWork = new InfluxDBUnitOfWork(MongoUoW);
            InfluxDBContext = InfluxDBUnitOfWork.Context;

        }

        [Test]
        public void MyFirstLog()
        {
            log.Information("My first try");
            Assert.IsTrue(true);
        }



    }
}
