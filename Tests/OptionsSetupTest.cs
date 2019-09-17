using AspNetCore.Yandex.ObjectStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class OptionsSetupTest
    {

        public IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }
        
        [TestMethod]
        public void TestOptionsRead()
        {
            var config = GetConfiguration();

            var options = config.GetYandexStorageOptions();

            Assert.IsTrue(options.BucketName == "your-bucket");
            Assert.IsTrue(options.Protocol == "http");
        }
        
        [TestMethod]
        public void InjectTest()
        {
            var config = GetConfiguration();
            IServiceCollection services = new ServiceCollection();
            
            services.AddYandexObjectStorage(config);
            
            var provider = services.BuildServiceProvider();
            var service = provider.GetService(typeof(YandexStorageService));
            
            Assert.IsNotNull(service, "service is null");
            Assert.IsInstanceOfType(service, typeof(YandexStorageService), "type incorrect");
            
            var optionsService = (IOptions<YandexStorageOptions>)provider.GetService(typeof(IOptions<YandexStorageOptions>));

            var yandexOptions = optionsService.Value;
            
            // Check options registration
            Assert.AreEqual("your-bucket", yandexOptions.BucketName, "Bucket name not configured");
            Assert.AreEqual("http", yandexOptions.Protocol, "Protocol not configured");
            Assert.AreEqual("your-access-key", yandexOptions.AccessKey, "Access Key not configured");
            Assert.AreEqual("your-secret-key", yandexOptions.SecretKey, "Secret Key not configured");
        }
    }
}