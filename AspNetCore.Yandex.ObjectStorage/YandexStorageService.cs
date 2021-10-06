using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Multipart;

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
		private static readonly HttpClient _client = new HttpClient();

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

		/// <summary>
		/// Test connection to storage
		/// </summary>
		/// <returns>Retruns true if all credentials correct</returns>
		public async Task<S3GetResponse> TryGetAsync()
		{
			var requestMessage = PrepareGetRequest();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			return new S3GetResponse(response);
		}

		public async Task<byte[]> GetAsByteArrayAsync(string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PrepareGetRequest(formattedPath);

			var response = await _client.SendAsync(requestMessage);

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

			var response = await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStreamAsync();
			}

			throw new Exception(await response.Content.ReadAsStringAsync());
		}

		public async Task<S3PutResponse> PutObjectAsync(Stream stream, string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PreparePutRequest(stream, formattedPath);

			var response = await _client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		public async Task<S3PutResponse> PutObjectAsync(byte[] byteArr, string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PreparePutRequest(byteArr, formattedPath);

			var response = await _client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		public async Task<S3DeleteResponse> DeleteObjectAsync(string filename)
		{
			var formattedPath = FormatPath(filename);

			var requestMessage = PrepareDeleteRequest(formattedPath);

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3DeleteResponse(response);
		}

		private string FormatPath(string path)
		{
			return path.RemoveProtocol(_protocol).RemoveEndPoint(_endpoint).RemoveBucket(_bucketName);
		}

		private HttpRequestMessage PrepareGetRequest()
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}"));
			var value = DateTime.UtcNow;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareGetRequest(string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			var value = DateTime.UtcNow;
			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PreparePutRequest(Stream stream, string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			var value = DateTime.UtcNow;
			ByteArrayContent content;
			if (stream is MemoryStream ms)
			{
				content = new ByteArrayContent(ms.ToArray());
			}
			else
			{
				using var memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				content = new ByteArrayContent(memoryStream.ToArray());
			}

			requestMessage.Content = content;
			stream.Dispose();

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PreparePutRequest(byte[] byteArr, string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			var value = DateTime.UtcNow;
			var content = new ByteArrayContent(byteArr);

			requestMessage.Content = content;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareDeleteRequest(string storageFileName)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{storageFileName}"));
			var value = DateTime.UtcNow;
			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}


		/// <summary>
		/// Multipart upload for files more than 100Mb
		/// </summary>
		/// <param name="stream">Stream with file</param>
		/// <param name="filename">Filename</param>
		/// <param name="partSize">Size of 1 part in Kb, must be more than 5120 Kb</param>
		/// <returns></returns>
		private async Task<string> MutipartAsync(Stream stream, string filename, int partSize = 6000)
		{
			var startResult = await StartUpload(filename);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Multipart upload for files more than 100Mb
		/// </summary>
		/// <param name="byteArr">File in byte array</param>
		/// <param name="filename">Filename</param>
		/// <param name="partSize">Size of 1 part in Kb, must be more than 5120 Kb</param>
		/// <returns></returns>
		private async Task<string> MutipartAsync(byte[] byteArr, string filename, int partSize = 6000)
		{
			// https://cloud.yandex.ru/docs/storage/s3/api-ref/multipart
			// Общий алгоритм

			// Ининциализируем начало загрузки - отправляем метаданные - получаем индефикатор загрузки

			// Разбиваем файл на части и указываем номера частей, а также присваеваем айди загрузки

			// Загружаем все части (по 1ой) асинхронно

			// Отправляем запрос на завершение загрузки

			var startResult = await StartUpload(filename);

			throw new NotImplementedException();
		}

		private async Task<bool> FileToParts(Stream stream, InitiateMultipartUploadResult startResponse, int partSize)
		{
			var part = new byte[partSize];
			var offset = 0;
			var partNumber = 0;
			while (stream.Position != stream.Length)
			{
				var position = await stream.ReadAsync(part, offset, partSize);
				offset += partSize;
				partNumber += 1;
				await UploadPart(part, partNumber, startResponse.UploadId);
				part = new byte[partSize];
			}

			return true;
		}

		private async Task<InitiateMultipartUploadResult> StartUpload(string filename)
		{
			throw new NotImplementedException();
		}

		private async Task<string> UploadPart(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> CopyPart(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> ListParts(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> AbortUpload(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> CompleteUpload(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> ListUploads(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private string GetObjectUri(string filename)
		{
			return $"{_hostName}/{filename}";
		}
	}
}