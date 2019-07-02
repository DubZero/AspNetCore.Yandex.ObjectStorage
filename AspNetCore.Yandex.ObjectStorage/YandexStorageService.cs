using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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
            
            //requestMessage.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
            requestMessage.Headers.Add("Host",_endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");
            //requestMessage.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            
            string[] headers = {"host","x-amz-content-sha256", "x-amz-date" };
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
            requestMessage.Headers.Add("Host",_endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");
            
            string[] headers = {"host","x-amz-content-sha256", "x-amz-date" };
            string signature = calculator.CalculateSignature(requestMessage, headers, value);
            string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";
            
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }
        
        private HttpRequestMessage PreparePutRequest(MemoryStream stream, string filename)
        {
            AwsV4SignatureCalculator calculator = new AwsV4SignatureCalculator(_secretKey);
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri($"{_protocol}://{_endpoint}/{_bucketName}/{filename}"));
            DateTime value = DateTime.UtcNow;
            
            var content = new ByteArrayContent(stream.ToArray());
            requestMessage.Content = content;
            stream.Dispose();
            
            requestMessage.Headers.Add("Host",_endpoint);
            requestMessage.Headers.Add("X-Amz-Content-Sha256", AwsV4SignatureCalculator.GetPayloadHash(requestMessage));
            requestMessage.Headers.Add("X-Amz-Date", $"{value:yyyyMMddTHHmmssZ}");
            
            string[] headers = {"host","x-amz-content-sha256", "x-amz-date" };
            string signature = calculator.CalculateSignature(requestMessage, headers, value);
            string authHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{value:yyyyMMdd}/us-east-1/s3/aws4_request, SignedHeaders={string.Join(";", headers)}, Signature={signature}";
            
            requestMessage.Headers.TryAddWithoutValidation("Authorization", authHeader);

            return requestMessage;
        }
        
        public HttpRequestMessage PrepareDeleteRequest(string storageFileName)
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

        public async Task<bool> TryGet()
        {
            var requestMessage = PrepareGetRequest();
            
            using (HttpClient client = new HttpClient())
            {
                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return true;
                }

                return false;
            }
        }

        public async Task<string> PutObject(MemoryStream stream, string filename)
        {
            var requestMessage = PreparePutRequest(stream, filename);
            
            using (HttpClient client = new HttpClient())
            {
                var response = await client.SendAsync(requestMessage);
                
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var fileResult = GetObjectUri(filename);
                    return fileResult;
                }

                return result;
            }
        }

        public async Task<bool> DeleteObject(string filename)
        {
            var requestMessage = PrepareDeleteRequest(filename);

            using (HttpClient client = new HttpClient())
            {
                var response = await client.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return true;
                }
                
                var error = await response.Content.ReadAsStringAsync();

                return false;
            }
        }
        
        private string GetObjectUri(string filename)
        {
            return $"{_hostName}/{filename}";
        }

    }
}