using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Builders
{
    internal class BucketListObjectsRequestBuilder
    {
        private readonly YandexStorageOptions _options;
        private HttpRequestMessage _request;

        internal BucketListObjectsRequestBuilder(YandexStorageOptions options)
        {
            _options = options;
        }

        internal async Task<BucketListObjectsRequestBuilder> BuildAsync(BucketListObjectsParameters parameters)
        {
            var url = $"{_options.Protocol}://{_options.Endpoint}/{parameters.BucketName}?{FormatParameters(parameters)}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url));
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

        private static string FormatParameters(BucketListObjectsParameters parameters)
        {
            var continueToken = string.IsNullOrEmpty(parameters.ContinueToken)
                ? string.Empty
                : $"&continuation-token={parameters.ContinueToken}";
            var delimiter = string.IsNullOrEmpty(parameters.Delimiter)
                ? string.Empty
                : $"&delimiter={parameters.Delimiter}";
            var maxKeys = $"&max-keys={parameters.MaxKeys}";
            var prefix = string.IsNullOrEmpty(parameters.Prefix)
                ? string.Empty
                : $"&prefix={parameters.Prefix}";
            var startAfter = string.IsNullOrEmpty(parameters.StartAfter)
                ? string.Empty
                : $"&start-after={parameters.StartAfter}";

            return string.Concat("list-type=2", continueToken, delimiter, maxKeys, prefix, startAfter);
        }
    }
}