using System;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Multipart.Builders;
using AspNetCore.Yandex.ObjectStorage.Multipart.Responses;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage.Multipart
{
    internal class MultipartUploadService : IMultipartUploadService
    {
        private readonly YandexStorageOptions _options;
        private readonly HttpClient _client;

        public MultipartUploadService(IOptions<YandexStorageOptions> options, HttpClient client)
        {
            _options = options.Value;
            _client = client;
        }

        public MultipartUploadService(YandexStorageOptions options, HttpClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task<S3InitiateMultipartUploadResult> StartUploadAsync(string filename)
        {
            var request = await new MultipartStartUploadRequestBuilder(_options).BuildAsync(filename);
            var response = await _client.SendAsync(request);

            return new S3InitiateMultipartUploadResult(response);
        }

        public async Task<string> UploadPartAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CopyPartAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ListPartsAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AbortUploadAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CompleteUploadAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ListUploadsAsync(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }
    }
}