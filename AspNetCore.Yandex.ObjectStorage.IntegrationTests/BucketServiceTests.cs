using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Models;
using Bogus;
using Xunit;

namespace AspNetCore.Yandex.ObjectStorage.IntegrationTests
{
    public class BucketServiceTests
    {
        private readonly Faker _faker;
        private readonly IYandexStorageService _yandexStorageService;
        private readonly IYandexStorageService _anotherLocationService;

        public BucketServiceTests()
        {
            _faker = new Faker("en");

            _yandexStorageService = new YandexStorageService(EnvironmentOptions.GetFromEnvironment());
            _anotherLocationService = new YandexStorageService(EnvironmentOptions.GetFromEnvironmentWithNotDefaultLocation());
        }
    }
}