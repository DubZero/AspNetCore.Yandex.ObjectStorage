using System;
using System.IO;
using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Builders.ObjectRequestBuilders
{
	internal class ObjectPutRequestBuilder
	{
		private readonly YandexStorageOptions _options;
		private HttpRequestMessage _request;
		
		internal ObjectPutRequestBuilder(YandexStorageOptions options)
		{
			_options = options;
		}
		
		internal ObjectPutRequestBuilder Build(Stream stream, string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_options.SecretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{filename}"));
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

			requestMessage.Headers.Add("Host", _options.Endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_options.SecretKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);
			_request = requestMessage;

			return this;
		}

		internal ObjectPutRequestBuilder Build(byte[] byteArr, string filename)
		{
			var calculator = new AwsV4SignatureCalculator(_options.SecretKey);
			var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_options.Protocol}://{_options.Endpoint}/{_options.BucketName}/{filename}"));
			var value = DateTime.UtcNow;
			var content = new ByteArrayContent(byteArr);

			requestMessage.Content = content;

			requestMessage.Headers.Add("Host", _options.Endpoint);
			requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
			requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

			string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
			var signature = calculator.CalculateSignature(requestMessage, headers, value);
			var authHeader = $"AWS4-HMAC-SHA256 Credential={_options.SecretKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

			requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);
			_request = requestMessage;

			return this;
		}
		
		internal HttpRequestMessage GetResult()
		{
			return _request;
		}
	}
}