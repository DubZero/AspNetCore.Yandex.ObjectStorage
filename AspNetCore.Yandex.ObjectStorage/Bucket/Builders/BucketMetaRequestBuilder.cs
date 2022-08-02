using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        internal async Task<BucketMetaRequestBuilder> BuildAsync(string bucketName)
        {
            var url = $"{_options.Protocol}://{_options.Endpoint}/{bucketName}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Head, new Uri(url));
            var dateAmz = DateTime.UtcNow;

            await requestMessage.AddBothHeadersAsync(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            await requestMessage.AddAuthHeaderAsync(_options, dateAmz, headers);
            _request = requestMessage;

            return this;
        }

        internal HttpRequestMessage GetResult()
        {
            return _request;
        }
    }
}