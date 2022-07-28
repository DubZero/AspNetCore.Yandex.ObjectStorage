using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;
using AspNetCore.Yandex.ObjectStorage.Models;
using FluentResults;
using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage.Object
{
    internal class ObjectService : IObjectService
    {
        private readonly string _protocol;
        private readonly string _bucketName;
        private readonly string _location;
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _hostName;
        private static readonly HttpClient _client = new HttpClient();

        public ObjectService(IOptions<YandexStorageOptions> options)
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

        public ObjectService(YandexStorageOptions options)
        {
            _protocol = options.Protocol;
            _bucketName = options.BucketName;
            _location = options.Location;
            _endpoint = options.Endpoint;
            _accessKey = options.AccessKey;
            _secretKey = options.SecretKey;
            _hostName = options.HostName;
        }

        public async Task<S3GetResponse> GetAsync(string filename)
        {
	        var formattedPath = FormatPath(filename);

	        var requestMessage = PrepareGetRequest(formattedPath);

	        var response = new S3GetResponse(await _client.SendAsync(requestMessage));

	        return response;
        }

        /// <summary>
        /// Return object as byte array
        /// </summary>
        /// <param name="filename">full URL or filename if it is in root folder</param>
        /// <returns>File object array</returns>
        public async Task<Result<byte[]>> GetAsByteArrayAsync(string filename)
        {
	        var formattedPath = FormatPath(filename);

	        var requestMessage = PrepareGetRequest(formattedPath);

	        var response = new S3GetResponse(await _client.SendAsync(requestMessage));

	        return await response.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Return object as Stream
        /// </summary>
        /// <param name="filename">full URL or filename if it is in root folder</param>
        /// <returns>Stream</returns>
        public async Task<Result<Stream>> GetAsStreamAsync(string filename)
        {
			var formattedPath = FormatPath(filename);
			var requestMessage = PrepareGetRequest(formattedPath);

			var response = new S3GetResponse(await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));

			return await response.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public async Task<S3PutResponse> PutAsync(Stream stream, string filename)
        {
	        var formattedPath = FormatPath(filename);

	        var requestMessage = PreparePutRequest(stream, formattedPath);

	        var response = await _client.SendAsync(requestMessage);

	        return new S3PutResponse(response, GetObjectUri(formattedPath));
        }

        public async Task<S3PutResponse> PutAsync(byte[] byteArr, string filename)
        {
	        var formattedPath = FormatPath(filename);

	        var requestMessage = PreparePutRequest(byteArr, formattedPath);

	        var response = await _client.SendAsync(requestMessage);

	        return new S3PutResponse(response, GetObjectUri(formattedPath));
        }

        public async Task<S3DeleteResponse> DeleteAsync(string filename)
        {
	        var formattedPath = FormatPath(filename);

	        var requestMessage = PrepareDeleteRequest(formattedPath);

	        var response = await _client.SendAsync(requestMessage);

	        return new S3DeleteResponse(response);
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

		private HttpRequestMessage PreparePutRequest(Stream stream, string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
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
			var headersString = string.Join(";", headers);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={headersString}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PreparePutRequest(byte[] byteArr, string filename)
		{
 			var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
			var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
			var value = DateTime.UtcNow;
			var content = new ByteArrayContent(byteArr);

			requestMessage.Content = content;

			requestMessage.Headers.Add("Host", _endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

			return requestMessage;
		}

		private HttpRequestMessage PrepareDeleteRequest(string storageFileName)
		{
			var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
			var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{storageFileName}"));
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
		private string GetObjectUri(string filename)
		{
			return $"{_hostName}/{filename}";
		}

		private string FormatPath(string path)
		{
			return path.RemoveProtocol(_protocol).RemoveEndPoint(_endpoint).RemoveBucket(_bucketName);
		}
    }
}