using System;
using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Builders.ObjectRequestBuilders
{
    internal class ObjectGetRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;
        
        internal ObjectGetRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }
        
        internal ObjectGetRequestBuilder Build()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}"));
            var dateAmz = DateTime.UtcNow;

            requestMessage.AddBothHeaders(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            requestMessage.AddAuthHeader(_options, dateAmz, headers);
            _request = requestMessage;

            return this;
        }
        
        internal ObjectGetRequestBuilder Build(string filename)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{filename}"));
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