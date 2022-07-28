using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Bucket;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage.Multipart;
using AspNetCore.Yandex.ObjectStorage.Object;
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
		private readonly string _hostName;
		private static readonly HttpClient _client = new HttpClient();

		public IObjectService ObjectService { get; }

		public YandexStorageService(IOptions<YandexStorageOptions> options)
		{
			var yandexStorageOptions = options.Value;
			ObjectService = new ObjectService(yandexStorageOptions);

			_protocol = yandexStorageOptions.Protocol;
			_bucketName = yandexStorageOptions.BucketName;
			_location = yandexStorageOptions.Location;
			_endpoint = yandexStorageOptions.Endpoint;
			_accessKey = yandexStorageOptions.AccessKey;
			_secretKey = yandexStorageOptions.SecretKey;
			_hostName = yandexStorageOptions.HostName;
		}

		public YandexStorageService(YandexStorageOptions options)
		{
			ObjectService = new ObjectService(options);
			_protocol = options.Protocol;
			_bucketName = options.BucketName;
			_location = options.Location;
			_endpoint = options.Endpoint;
			_accessKey = options.AccessKey;
			_secretKey = options.SecretKey;
			_hostName = options.HostName;
		}

		public async Task<S3GetResponse> TryConnectAsync()
		{
			var requestMessage = PrepareGetRequest();

			var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			return new S3GetResponse(response);
		}

		/// <summary>
		/// Test connection to storage
		/// </summary>
		/// <returns>Retruns true if all credentials correct</returns>
		public async Task<S3GetResponse> TryGetAsync(string filename)
		{
			var requestMessage = PrepareGetRequest(filename);

			var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			return new S3GetResponse(response);
		}

		private HttpRequestMessage PrepareGetRequest()
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}"));
			var value = DateTime.UtcNow;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareGetRequest(string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			var value = DateTime.UtcNow;
			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}
	}
}