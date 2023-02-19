using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;
using AspNetCore.Yandex.ObjectStorage.Object.Parameters;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

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
        private readonly HttpClient _client;
        private readonly Version _httpRequestVersion;

        public ObjectService(IOptions<YandexStorageOptions> options, HttpClient client)
        {
            var yandexStorageOptions = options.Value;

            _client = client;
            _protocol = yandexStorageOptions.Protocol;
            _bucketName = yandexStorageOptions.BucketName;
            _location = yandexStorageOptions.Location;
            _endpoint = yandexStorageOptions.Endpoint;
            _accessKey = yandexStorageOptions.AccessKey;
            _secretKey = yandexStorageOptions.SecretKey;
            _hostName = yandexStorageOptions.HostName;
            _httpRequestVersion = yandexStorageOptions.UseHttp2 ? new Version(2, 0) : new Version(1, 1);
        }

        public ObjectService(YandexStorageOptions options, HttpClient client)
        {
            _protocol = options.Protocol;
            _bucketName = options.BucketName;
            _location = options.Location;
            _endpoint = options.Endpoint;
            _accessKey = options.AccessKey;
            _secretKey = options.SecretKey;
            _hostName = options.HostName;
            _client = client;
            _httpRequestVersion = options.UseHttp2 ? new Version(2, 0) : new Version(1, 1);
        }

        public async Task<S3ObjectGetResponse> GetAsync(string filename)
        {
            var formattedPath = FormatPath(filename);
            var requestMessage = await PrepareGetRequestAsync(formattedPath);
            var response = new S3ObjectGetResponse(await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));

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

            var requestMessage = await PrepareGetRequestAsync(formattedPath);

            var response = new S3ObjectGetResponse(await _client.SendAsync(requestMessage));

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
            var requestMessage = await PrepareGetRequestAsync(formattedPath);

            var response = new S3ObjectGetResponse(await _client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead));

            return await response.ReadAsStreamAsync().ConfigureAwait(false);
        }

        public async Task<S3ObjectPutResponse> PutAsync(Stream stream, string filename)
        {
            var formattedPath = FormatPath(filename);

            var requestMessage = await PreparePutRequestAsync(stream, formattedPath);

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectPutResponse(response, GetObjectUri(formattedPath));
        }

        public async Task<S3ObjectPutResponse> PutAsync(byte[] byteArr, string filename)
        {
            var formattedPath = FormatPath(filename);

            var requestMessage = await PreparePutRequestAsync(byteArr, formattedPath);

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectPutResponse(response, GetObjectUri(formattedPath));
        }

        public async Task<S3ObjectDeleteResponse> DeleteAsync(string filename)
        {
            var formattedPath = FormatPath(filename);

            var requestMessage = await PrepareDeleteRequestAsync(formattedPath);

            var response = await _client.SendAsync(requestMessage);

            return new S3ObjectDeleteResponse(response);
        }

        public async Task<S3MultipleObjectDeleteResponse> DeleteMultipleAsync(DeleteMultipleObjectsParameters parameters)
        {
            var requestMessage = await PrepareDeleteMultipleRequestAsync(parameters);

            var response = await _client.SendAsync(requestMessage);

            return new S3MultipleObjectDeleteResponse(response);
        }

        private async Task<HttpRequestMessage> PrepareGetRequestAsync(string filename)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"))
            {
                Version = _httpRequestVersion
            };
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

        private async Task<HttpRequestMessage> PreparePutRequestAsync(Stream stream, string filename)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"))
            {
                Version = _httpRequestVersion
            };
            var value = DateTime.UtcNow;
            ByteArrayContent content;
            if (stream is MemoryStream ms)
            {
                content = new ByteArrayContent(ms.ToArray());
            }
            else
            {
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                content = new ByteArrayContent(memoryStream.ToArray());
            }

            requestMessage.Content = content;
            await stream.DisposeAsync();

            requestMessage.Headers.Add("Host", _endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = await calculator.CalculateSignatureAsync(requestMessage, headers, value);
            var headersString = string.Join(";", headers);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={headersString}, Signature={signature}";

            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }

        private async Task<HttpRequestMessage> PreparePutRequestAsync(byte[] byteArr, string filename)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"))
            {
                Version = _httpRequestVersion
            };
            var value = DateTime.UtcNow;
            var content = new ByteArrayContent(byteArr);

            requestMessage.Content = content;

            requestMessage.Headers.Add("Host", _endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = await calculator.CalculateSignatureAsync(requestMessage, headers, value);
            var authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/{_location}/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";

            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }

        private async Task<HttpRequestMessage> PrepareDeleteRequestAsync(string storageFileName)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{storageFileName}"))
            {
                Version = _httpRequestVersion
            };
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

        private async Task<HttpRequestMessage> PrepareDeleteMultipleRequestAsync(DeleteMultipleObjectsParameters parameters)
        {
            var calculator = new AwsV4SignatureCalculator(_secretKey, _location);
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri($"{_protocol}://{_endpoint}/{_bucketName}?delete"))
            {
                Version = _httpRequestVersion
            };
            var value = DateTime.UtcNow;

            var xmlSerializer = new XmlSerializer(typeof(DeleteMultipleObjectsParameters));
            StringContent content;
            using (var ms = new MemoryStream())
            using (var tw = new XmlTextWriter(ms, Encoding.UTF8))
            {
                tw.Formatting = Formatting.Indented;
                xmlSerializer.Serialize(tw, parameters);

                content = new StringContent(Encoding.UTF8.GetString(ms.ToArray()), Encoding.UTF8, "text/xml");
            }

            requestMessage.Content = content;

            requestMessage.Headers.Add("Host", _endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", await AwsV4SignatureCalculator.GetPayloadHashAsync(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");

            string[] headers = { "host", "x-amz-content-sha256", "x-amz-date" };
            var signature = await calculator.CalculateSignatureAsync(requestMessage, headers, value);
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