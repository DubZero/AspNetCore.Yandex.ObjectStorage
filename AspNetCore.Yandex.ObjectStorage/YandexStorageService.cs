using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage
{
	public class YandexStorageService
	{
		private readonly string _protocol;
		private readonly string _bucketName;
		private readonly string _location;
		private readonly string _endpoint;
		private readonly string _accessKey;
		private readonly string _secretKey;
		private readonly string _hostName;

		public YandexStorageService(IOptions<YandexStorageOptions> options)
		{
			var yandexStorageOptions = options.Value;

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
			_protocol = options.Protocol;
			_bucketName = options.BucketName;
			_location = options.Location;
			_endpoint = options.Endpoint;
			_accessKey = options.AccessKey;
			_secretKey = options.SecretKey;
			_hostName = options.HostName;
		}

		private HttpRequestMessage PrepareGetRequest()
		{
			AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}"));
			DateTime value = DateTime.UtcNow;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			string signature = calculator.CalculateSignature(requestMessage, headers, value);
			string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareGetRequest(string filename)
		{
			AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			DateTime value = DateTime.UtcNow;
			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			string signature = calculator.CalculateSignature(requestMessage, headers, value);
			string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PreparePutRequest(Stream stream, string filename)
		{
			AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			DateTime value = DateTime.UtcNow;
			ByteArrayContent content;
			if (stream is MemoryStream ms)
			{
				content = new ByteArrayContent(ms.ToArray());
			}
			else
			{
				using MemoryStream memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				content = new ByteArrayContent(memoryStream.ToArray());
			}

			requestMessage.Content = content;
			stream.Dispose();

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			string signature = calculator.CalculateSignature(requestMessage, headers, value);
			string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PreparePutRequest(byte[] byteArr, string filename)
		{
			AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			DateTime value = DateTime.UtcNow;
			ByteArrayContent content = new ByteArrayContent(byteArr);

			requestMessage.Content = content;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			string signature = calculator.CalculateSignature(requestMessage, headers, value);
			string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareDeleteRequest(string storageFileName)
		{
			AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{storageFileName}"));
			DateTime value = DateTime.UtcNow;
			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			string signature = calculator.CalculateSignature(requestMessage, headers, value);
			string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		/// <summary>
		/// Test connection to storage
		/// </summary>
		/// <returns>Retruns true if all credentials correct</returns>
		public async Task<S3GetResponse> TryGetAsync()
		{
			var requestMessage = PrepareGetRequest();

			using HttpClient client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3GetResponse(response);
		}

		public async Task<byte[]> GetAsByteArrayAsync(string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PrepareGetRequest(formattedPath);

			using HttpClient client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsByteArrayAsync();
			}

			throw new Exception(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Return object as Stream
		/// </summary>
		/// <param name="filename">full URL or filename if it is in root folder</param>
		/// <returns>Stream</returns>
		/// <exception cref="Exception"></exception>
		public async Task<Stream> GetAsStreamAsync(string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PrepareGetRequest(formattedPath);

			using HttpClient client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStreamAsync();
			}

			throw new Exception(await response.Content.ReadAsStringAsync());
		}

		public async Task<S3Response> PutObjectAsync(Stream stream, string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PreparePutRequest(stream, formattedPath);

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		public async Task<S3Response> PutObjectAsync(byte[] byteArr, string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PreparePutRequest(byteArr, formattedPath);

			using HttpClient client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		private string FormatPath(string path)
		{
			return path.RemoveProtocol(_protocol).RemoveEndPoint(_endpoint).RemoveBucket(_bucketName);
		}

		public async Task<S3DeleteResponse> DeleteObjectAsync(string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PrepareDeleteRequest(formattedPath);

			using HttpClient client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3DeleteResponse(response);
		}

		private string GetObjectUri(string filename)
		{
			return $"{_hostName}/{filename}";
		}
	}
}