using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class RequestTest
    {
        [TestMethod]
        public void TestEnvVariables()
        {
            var secret = Environment.GetEnvironmentVariable("SecretKey");
            var bucket = Environment.GetEnvironmentVariable("Bucket");
            var accessKey = Environment.GetEnvironmentVariable("AccessKey");
            
            Assert.IsNotNull(secret);
            Assert.IsNotNull(bucket);
            Assert.IsNotNull(accessKey);
        }
    }
}