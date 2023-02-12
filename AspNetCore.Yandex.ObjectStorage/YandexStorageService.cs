using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Object;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage
{
    public class YandexStorageService : IYandexStorageService
    {
        private readonly string _protocol;
        private readonly string _bucketName;
        private readonly string _location;
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly HttpClient _client;

        public IObjectService ObjectService { get; }
        public IBucketService BucketService { get; }

        public YandexStorageService(IOptions<YandexStorageOptions> options, HttpClient client)
        {
            var yandexStorageOptions = options.Value;
            ObjectService = new ObjectService(yandexStorageOptions);
            BucketService = new BucketService(yandexStorageOptions);
            _client = client;
            _protocol = yandexStorageOptions.Protocol;
            _bucketName = yandexStorageOptions.BucketName;
            _location = yandexStorageOptions.Location;
            _endpoint = yandexStorageOptions.Endpoint;
            _accessKey = yandexStorageOptions.AccessKey;
            _secretKey = yandexStorageOptions.SecretKey;
        }

        public YandexStorageService(YandexStorageOptions options)
        {
            ObjectService = new ObjectService(options);
            BucketService = new BucketService(options);
            _protocol = options.Protocol;
            _bucketName = options.BucketName;
            _location = options.Location;
            _endpoint = options.Endpoint;
            _accessKey = options.AccessKey;
            _secretKey = options.SecretKey;
        }

        public async Task<S3ObjectGetResponse> TryConnectAsync()
        {
            var requestMessage = await PrepareGetRequestAsync();

            var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            return new S3ObjectGetResponse(response);
        }

        /// <summary>
        /// Test connection to storage
        /// </summary>
        /// <returns>Retruns true if all credentials correct</returns>
        public async Task<S3ObjectGetResponse> TryGetAsync(string filename)
        {
            var requestMessage = await PrepareGetRequestAsync(filename);

            var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            return new S3ObjectGetResponse(response);
        }

        private async Task<HttpRequestMessage> PrepareGetRequestAsync()
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}"));
            var value = DateTime.UtcNow;

            requestMessage.Headers.Add("Host", _endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = await calculator.CalculateSignatureAsync(requestMessage, headers, value);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }

        private async Task<HttpRequestMessage> PrepareGetRequestAsync(string filename)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
            var value = DateTime.UtcNow;
            requestMessage.Headers.Add("Host", _endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = await calculator.CalculateSignatureAsync(requestMessage, headers, value);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }
    }
}