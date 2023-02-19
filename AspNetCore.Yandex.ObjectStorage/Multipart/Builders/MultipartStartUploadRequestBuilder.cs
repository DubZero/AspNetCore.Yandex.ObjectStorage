using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Multipart.Builders
{
    internal class MultipartStartUploadRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private readonly Version _httpRequestVersion;

        internal MultipartStartUploadRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
            _httpRequestVersion = options.UseHttp2 ? new Version(2, 0) : new Version(1, 1);
        }

        internal async Task<HttpRequestMessage> BuildAsync(string filename)
        {
            var uri = new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{filename}?uploads");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri)
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