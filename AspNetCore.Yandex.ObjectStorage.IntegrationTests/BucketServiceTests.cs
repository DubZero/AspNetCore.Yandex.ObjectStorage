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

        public BucketServiceTests()
        {
            _faker = new Faker("en");

            _yandexStorageService = new YandexStorageService(EnvironmentOptions.GetFromEnvironment());
        }


        [Fact(DisplayName = "[001] CreateBucket test")]
        public async Task CreateBucket_Success()
        {
            var bucketName = _faker.Random.String2(10);

            var result = await _yandexStorageService.BucketService.CreateBucket(bucketName);

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var createResult = await result.ReadResultAsStringAsync();

            Assert.True(createResult.IsSuccess);

            await DeleteBucketAsync(bucketName);
        }


        [Fact(DisplayName = "[002] Delete Bucket test")]
        public async Task DeleteBucket_Success()
        {
            var bucketName = _faker.Random.String2(10);

            var result = await _yandexStorageService.BucketService.CreateBucket(bucketName);

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var response = await _yandexStorageService.BucketService.DeleteBucket(bucketName);

            var deleteResult = await response.ReadResultAsStringAsync();

            Assert.True(deleteResult.IsSuccess);
        }

        private async Task DeleteBucketAsync(string bucketName)
        {
            await _yandexStorageService.BucketService.CreateBucket(bucketName);
        }
    }
}