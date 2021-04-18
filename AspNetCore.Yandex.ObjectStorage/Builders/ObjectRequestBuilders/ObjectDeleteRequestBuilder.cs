using System;
using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Builders.ObjectRequestBuilders
{
    internal class ObjectDeleteRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;
        
        internal ObjectDeleteRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }
        
        internal ObjectDeleteRequestBuilder Build(string storageFileName)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{storageFileName}"));
            var dateAmz = DateTime.UtcNow;

            requestMessage.AddBothHeaders(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            requestMessage.AddAuthHeader(_options, dateAmz, headers);
            _request = requestMessage;

            return this;
        }

        public HttpRequestMessage GetResult()
        {
            return _request;
        }
    }
}