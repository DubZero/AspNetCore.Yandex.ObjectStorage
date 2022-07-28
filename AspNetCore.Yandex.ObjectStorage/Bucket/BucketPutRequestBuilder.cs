using System;
using System.Net.Http;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Bucket
{
    internal class BucketPutRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;

        internal BucketPutRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }

        internal BucketPutRequestBuilder Build(BucketCreateOptions createModel)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_options.Protocol}://{_options.Endpoint}/{createModel.BucketName}"));
            var dateAmz = DateTime.UtcNow;

            requestMessage.AddBothHeaders(_options, dateAmz);
            requestMessage.Headers.Add("x-amz-acl", createModel.ACLType.GetACLHeaderValue());

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date", "x-amz-acl" };
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