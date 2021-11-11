using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

			var optionsService = (IOptions<YandexStorageOptions>)provider.GetService(typeof(IOptions<YandexStorageOptions>));

			var yandexOptions = optionsService.Value;

			// Check options registration
			Assert.AreEqual("bucket", yandexOptions.BucketName, "Bucket name not configured");
			Assert.AreEqual("location", yandexOptions.Location, "Location not configured");
			Assert.AreEqual("aKey", yandexOptions.AccessKey, "Access Key not configured");
			Assert.AreEqual("sKey", yandexOptions.SecretKey, "Secret Key not configured");
			Assert.AreEqual("endpoint", yandexOptions.Endpoint, "EndPoint not configured");
		}
	}
}