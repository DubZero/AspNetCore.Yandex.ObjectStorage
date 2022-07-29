using System;
using System.Net.Http;
using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Builders
{
    internal class BucketMetaRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;

        internal BucketMetaRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }

        internal BucketMetaRequestBuilder Build(string bucketName)
        {
            var url = $"{_options.Protocol}://{_options.Endpoint}/{bucketName}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Head, new Uri(url));
            var dateAmz = DateTime.UtcNow;

            requestMessage.AddBothHeaders(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
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