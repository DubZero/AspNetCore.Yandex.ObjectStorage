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
        private readonly HttpClient _client;

        public BucketService(IOptions<YandexStorageOptions> options, HttpClient client)
        {
            _options = options.Value;
            _client = client;
        }

        public BucketService(YandexStorageOptions options, HttpClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task<S3ObjectPutResponse> CreateAsync(string bucketName)
        {
            var requestMessage = await new BucketPutRequestBuilder(_options).BuildAsync(bucketName);

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectPutResponse(response, GetBucketUri(bucketName));
        }

        public async Task<S3Response> GetBucketMetaAsync(string bucketName)
        {
            var requestMessage = await new BucketMetaRequestBuilder(_options).BuildAsync(bucketName);

            var response = await _client.SendAsync(requestMessage);

            return new S3Response(response);
        }

        public async Task<S3BucketObjectListResponse> GetBucketListObjectsAsync(BucketListObjectsParameters parameters)
        {
            var requestMessage = await new BucketListObjectsRequestBuilder(_options).BuildAsync(parameters);
            var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            return new S3BucketObjectListResponse(response);
        }

        public async Task<S3BucketListResponse> GetAllAsync()
        {
            var requestMessage = await new BucketListRequestBuilder(_options).BuildAsync();

            var response = await _client.SendAsync(requestMessage);

            return new S3BucketListResponse(response);
        }

        public async Task<S3ObjectDeleteResponse> DeleteAsync(string bucketName)
        {
            var requestMessage = await new BucketDeleteRequestBuilder(_options).BuildAsync(bucketName);

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectDeleteResponse(response);
        }

        private string GetBucketUri(string bucket)
        {
            return $"{_options.Protocol}/{_options.Endpoint}/{bucket}";
        }
    }
}