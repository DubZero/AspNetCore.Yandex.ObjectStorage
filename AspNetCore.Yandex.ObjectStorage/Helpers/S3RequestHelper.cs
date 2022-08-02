using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Configuration;

namespace AspNetCore.Yandex.ObjectStorage.Helpers
{
    internal static class S3RequestHelper
    {
        internal static async Task AddBothHeadersAsync(this HttpRequestMessage request, YandexStorageOptions options, DateTime dateAmz)
        {
            (await request.AddHost(options.Endpoint)
                .AddPayloadHashAsync())
                .AddAmzDate(dateAmz);
        }

        internal static async Task AddAuthHeaderAsync(this HttpRequestMessage request, YandexStorageOptions options,
            DateTime dateAmz, string[] headers)
        {
            var calculator = new AwsV4SignatureCalculator(options.SecretKey, options.Location);
            var signature = await calculator.CalculateSignatureAsync(request, headers, dateAmz);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={options.AccessKey}/{dateAmz:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

            request.Headers.TryAddWithoutValidation("Authorization", authHeader);
        }

        private static HttpRequestMessage AddHost(this HttpRequestMessage request, string endpoint)
        {
            request.Headers.Add("Host", endpoint);

            return request;
        }

        private static async Task<HttpRequestMessage> AddPayloadHashAsync(this HttpRequestMessage request)
        {
            request.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(request));

            return request;
        }

        private static HttpRequestMessage AddAmzDate(this HttpRequestMessage request, DateTime dateAmz)
        {
            request.Headers.Add("X-Amz-Date", $"{dateAmz:yyyyMMddTHHmmssZ}");

            return request;
        }
    }
}