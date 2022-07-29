using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Bucket.Builders;
using AspNetCore.Yandex.ObjectStorage.Bucket.Models;
using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Bucket.Responses;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Enums;
using AspNetCore.Yandex.ObjectStorage.Models;
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

        public async Task<S3PutResponse> CreateBucket(string bucketName)
        {
            var builder = new BucketPutRequestBuilder(_options);
            var requestMessage = builder.Build(bucketName).GetResult();

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            return new S3PutResponse(response, GetBucketUri(bucketName));
        }

        public Task<S3GetResponse> GetBucketMeta(string bucketName)
        {
            throw new NotImplementedException();
        }

        public async Task<S3BucketObjectListResponse> GetBucketListObjects(BucketListObjectsParameters parameters)
        {
            var builder = new BucketListObjectsRequestBuilder(_options);
            var requestMessage = builder.Build(parameters).GetResult();

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            return new S3BucketObjectListResponse(response);
        }

        public async Task<S3BucketListResponse> GetBucketList()
        {
            var requestMessage = new BucketListRequestBuilder(_options)
                .Build()
                .GetResult();

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            return new S3BucketListResponse(response);
        }

        public async Task<S3DeleteResponse> DeleteBucket(string bucketName)
        {
            var builder = new BucketDeleteRequestBuilder(_options);
            var requestMessage = builder.Build(bucketName).GetResult();

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            return new S3DeleteResponse(response);
        }

        private string GetBucketUri(string bucket)
        {
            return $"{_options.Protocol}/{_options.Endpoint}/{bucket}";
        }
    }
}