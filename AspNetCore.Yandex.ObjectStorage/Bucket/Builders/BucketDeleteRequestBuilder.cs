using System;
using System.Net.Http;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Builders
{
    internal class BucketDeleteRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;

        internal BucketDeleteRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }

        internal BucketDeleteRequestBuilder Build(string bucketName)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_options.Protocol}://{_options.Endpoint}/{bucketName}"));
            var dateAmz = DateTime.UtcNow;

            requestMessage.AddBothHeaders(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date"  };
            requestMessage.AddAuthHeader(_options, dateAmz, headers);
            _request = requestMessage;

            return this;
        }

        internal HttpRequestMessage GetResult()
        {
            return _request;
        }
    }
}