using AspNetCore.Yandex.ObjectStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class InjectTest
    {
        [TestMethod]
        public void TestOptionsSetup()
        {   
            IServiceCollection services = new ServiceCollection();

            services.AddYandexObjectStorage(options =>
            {
                options.BucketName = "bucket";
                options.Location = "location";
                options.AccessKey = "aKey";
                options.SecretKey = "sKey";
                options.Endpoint = "endpoint";
            });
            
            var provider = services.BuildServiceProvider();
            var service = provider.GetService(typeof(YandexStorageService));
            
            Assert.IsNotNull(service, "service is null");
            Assert.IsInstanceOfType(service, typeof(YandexStorageService), "type incorrect");
            
            // Check options

//            var yandexObjService = (YandexStorageService) service;
//            yandexObjService.

        }
    }
}
