using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;

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

        [Fact(DisplayName = "[003] List bucket objects")]
        public async Task ListBucketObjects_Success()
        {
            const string bucketName = "testbucketlib";

            var result = await _yandexStorageService.BucketService.GetBucketListObjects(new BucketListObjectsParameters()
            {
                BucketName = bucketName
            });

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var listObjectsResult = await result.ReadResultAsync();

            Assert.True(listObjectsResult.IsSuccess);

            var listObjects = listObjectsResult.Value;

            Assert.Equal(2, listObjects.Contents.Count);
        }


        [Fact(DisplayName = "[004] Bucket list")]
        public async Task BucketList_Success()
        {
            var result = await _yandexStorageService.BucketService.GetBucketList();

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var bucketListResult = await result.ReadResultAsync();

            Assert.True(bucketListResult.IsSuccess);

            var bucketList = bucketListResult.Value;

            Assert.NotEmpty(bucketList.Buckets);
            Assert.True(bucketList.Buckets.Any(p => p.Name == "testbucketlib"));
        }


        private async Task DeleteBucketAsync(string bucketName)
        {
            await _yandexStorageService.BucketService.CreateBucket(bucketName);
        }
    }
}