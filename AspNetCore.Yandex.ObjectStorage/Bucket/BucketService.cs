using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket.Builders;
using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Bucket.Responses;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage.Bucket
{
    internal class BucketService : IBucketService
    {
        private readonly YandexStorageOptions _options;
        private static readonly HttpClient _client = new HttpClient();

        public BucketService(IOptions<YandexStorageOptions> options)
        {
            _options = options.Value;
        }

        public BucketService(YandexStorageOptions options)
        {
            _options = options;
        }

        public async Task<S3ObjectPutResponse> CreateBucket(string bucketName)
        {
            var builder = new BucketPutRequestBuilder(_options);
            await builder.BuildAsync(bucketName);
            var requestMessage = builder.GetResult();

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectPutResponse(response, GetBucketUri(bucketName));
        }

        public async Task<S3Response> GetBucketMeta(string bucketName)
        {
            var builder = await new BucketMetaRequestBuilder(_options).BuildAsync(bucketName);

            var requestMessage = builder.GetResult();

            var response = await _client.SendAsync(requestMessage);

            return new S3Response(response);
        }

        public async Task<S3BucketObjectListResponse> GetBucketListObjects(BucketListObjectsParameters parameters)
        {
            var builder = await new BucketListObjectsRequestBuilder(_options).BuildAsync(parameters);
            var requestMessage = builder.GetResult();

            var response = await _client.SendAsync(requestMessage);

            return new S3BucketObjectListResponse(response);
        }

        public async Task<S3BucketListResponse> GetBucketList()
        {
            var builder = await new BucketListRequestBuilder(_options).BuildAsync();
            var requestMessage = builder.GetResult();

            var response = await _client.SendAsync(requestMessage);

            return new S3BucketListResponse(response);
        }

        public async Task<S3ObjectDeleteResponse> DeleteBucket(string bucketName)
        {
            var builder = await new BucketDeleteRequestBuilder(_options).BuildAsync(bucketName);
            var requestMessage = builder.GetResult();

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectDeleteResponse(response);
        }

        private string GetBucketUri(string bucket)
        {
            return $"{_options.Protocol}/{_options.Endpoint}/{bucket}";
        }
    }
}