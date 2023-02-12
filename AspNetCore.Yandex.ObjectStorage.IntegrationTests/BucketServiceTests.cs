using System.Linq;
using System.Net;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;

using Bogus;

using Xunit;

namespace AspNetCore.Yandex.ObjectStorage.IntegrationTests;

public class BucketServiceTests
{
    private readonly Faker _faker;
    private readonly IYandexStorageService _yandexStorageService;

    public BucketServiceTests()
    {
        _faker = new Faker("en");

        _yandexStorageService = new YandexStorageService(EnvironmentOptions.GetFromEnvironment());
    }


    [Fact(DisplayName = "[001] CreateAsync - adding new bucket with random valid name.")]
    public async Task CreateBucket_ValidBucketName_SuccessStatusCode()
    {
        var bucketName = _faker.Random.String2(10);

        var result = await _yandexStorageService.BucketService.CreateAsync(bucketName);

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var createResult = await result.ReadResultAsStringAsync();

        Assert.True(createResult.IsSuccess);

        await DeleteBucketAsync(bucketName);
    }


    [Fact(DisplayName = "[002] DeleteAsync - deleting existing bucket.")]
    public async Task DeleteBucket_ExistingBucket_SuccessStatusCode()
    {
        var bucketName = _faker.Random.String2(10);

        var result = await _yandexStorageService.BucketService.CreateAsync(bucketName);

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var response = await _yandexStorageService.BucketService.DeleteAsync(bucketName);

        var deleteResult = await response.ReadResultAsStringAsync();

        Assert.True(deleteResult.IsSuccess);
    }

    [Fact(DisplayName = "[003] GetBucketListObjectsAsync - get list of objects in test bucket. ")]
    public async Task GetBucketListObjects_ExistingBucket_SuccessStatusCode()
    {
        const string bucketName = "testbucketlib";

        var result = await _yandexStorageService.BucketService.GetBucketListObjectsAsync(new BucketListObjectsParameters()
        {
            BucketName = bucketName,
        });

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var listObjectsResult = await result.ReadResultAsync();

        Assert.True(listObjectsResult.IsSuccess);
    }

    [Fact(DisplayName = "[004] GetBucketListObjectsAsync - check object count in test bucket.")]
    public async Task GetBucketListObjects_WithTwoFiles_ObjectCountEqualsTwo()
    {
        const string bucketName = "testbucketlib";

        var result = await _yandexStorageService.BucketService.GetBucketListObjectsAsync(new BucketListObjectsParameters()
        {
            BucketName = bucketName
        });

        Assert.True(result.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);

        var listObjectsResult = await result.ReadResultAsync();
        var listObjects = listObjectsResult.Value;

        Assert.Equal(2, listObjects.Contents.Count);
    }


    [Fact(DisplayName = "[005] GetAllAsync - list all buckets.")]
    public async Task GetAll_TestBucketExists_ContentHaveTestBucket()
    {
        var result = await _yandexStorageService.BucketService.GetAllAsync();

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
        await _yandexStorageService.BucketService.DeleteAsync(bucketName);
    }
}