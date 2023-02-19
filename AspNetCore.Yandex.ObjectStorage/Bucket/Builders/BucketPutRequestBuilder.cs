using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Builders
{
    internal class BucketPutRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private readonly Version _httpRequestVersion;

        internal BucketPutRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
            _httpRequestVersion = options.UseHttp2 ? new Version(2, 0) : new Version(1, 1);
        }

        internal async Task<HttpRequestMessage> BuildAsync(string bucketName)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_options.Protocol}://{_options.Endpoint}/{bucketName}"))
            {
                Version = _httpRequestVersion
            };
            var dateAmz = DateTime.UtcNow;

            await requestMessage.AddBothHeadersAsync(_options, dateAmz);

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            await requestMessage.AddAuthHeaderAsync(_options, dateAmz, headers);

            return requestMessage;
        }
    }
}