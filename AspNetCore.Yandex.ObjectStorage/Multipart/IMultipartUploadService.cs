using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCore.Yandex.ObjectStorage.Multipart
{
    public interface IMultipartUploadService
    {
        /// <summary>
        /// Multipart upload for files more than 100Mb
        /// </summary>
        /// <param name="stream">Stream with file</param>
        /// <param name="filename">Filename</param>
        /// <param name="partSize">Size of 1 part in Kb, must be more than 5120 Kb</param>
        /// <returns></returns>
        public async Task<string> MultipartAsync(Stream stream, string filename, int partSize = 6000)
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
        public async Task<string> MultipartAsync(byte[] byteArr, string filename, int partSize = 6000)
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

        public async Task<bool> FileToParts(Stream stream, InitiateMultipartUploadResult startResponse, int partSize)
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

        public Task<InitiateMultipartUploadResult> StartUpload(string filename);

        public Task<string> UploadPart(byte[] filePart, int partNumber, string uploadId);

        public Task<string> CopyPart(byte[] filePart, int partNumber, string uploadId);

        public Task<string> ListParts(byte[] filePart, int partNumber, string uploadId);

        public Task<string> AbortUpload(byte[] filePart, int partNumber, string uploadId);

        public Task<string> CompleteUpload(byte[] filePart, int partNumber, string uploadId);

        public Task<string> ListUploads(byte[] filePart, int partNumber, string uploadId);
    }

    internal class MultipartUploadService : IMultipartUploadService
    {
        public async Task<InitiateMultipartUploadResult> StartUpload(string filename)
        {
            throw new NotImplementedException();
        }

        public async Task<string> UploadPart(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CopyPart(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ListParts(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> AbortUpload(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> CompleteUpload(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ListUploads(byte[] filePart, int partNumber, string uploadId)
        {
            throw new NotImplementedException();
        }
    }
}