using System;
using System.IO;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Multipart.Responses;

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
            var startResult = await StartUploadAsync(filename);

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

            var startResult = await StartUploadAsync(filename);

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
                await UploadPartAsync(part, partNumber, startResponse.UploadId);
                part = new byte[partSize];
            }

            return true;
        }

        public Task<S3InitiateMultipartUploadResult> StartUploadAsync(string filename);

        public Task<string> UploadPartAsync(byte[] filePart, int partNumber, string uploadId);

        public Task<string> CopyPartAsync(byte[] filePart, int partNumber, string uploadId);

        public Task<string> ListPartsAsync(byte[] filePart, int partNumber, string uploadId);

        public Task<string> AbortUploadAsync(byte[] filePart, int partNumber, string uploadId);

        public Task<string> CompleteUploadAsync(byte[] filePart, int partNumber, string uploadId);

        public Task<string> ListUploadsAsync(byte[] filePart, int partNumber, string uploadId);
    }
}