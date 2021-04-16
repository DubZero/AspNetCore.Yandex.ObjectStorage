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
            var calculator = new AwsV4SignatureCalculator(_options.SecretKey);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{storageFileName}"));
            var value = DateTime.UtcNow;
            requestMessage.Headers.Add("Host", _options.Endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = calculator.CalculateSignature(requestMessage, headers, value);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={_options.AccessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);
            _request = requestMessage;

            return this;
        }

        public HttpRequestMessage GetResult()
        {
            return _request;
        }
    }
}