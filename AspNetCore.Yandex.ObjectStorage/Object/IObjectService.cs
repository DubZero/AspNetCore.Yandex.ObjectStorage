using System;
using System.IO;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Object.Parameters;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Object
{
    public interface IObjectService
    {
        Task<S3ObjectGetResponse> GetAsync(string filename);

        Task<Result<byte[]>> GetAsByteArrayAsync(string filename);

        /// <summary>
        /// Return object as Stream
        /// </summary>
        /// <param name="filename">full URL or filename if it is in root folder</param>
        /// <returns>Stream</returns>
        /// <exception cref="Exception"></exception>
        Task<Result<Stream>> GetAsStreamAsync(string filename);

        /// <summary>
        /// https://cloud.yandex.ru/docs/storage/s3/api-ref/object/upload
        /// </summary>
        /// <param name="stream">Put ob</param>
        /// <param name="filename"></param>
        Task<S3ObjectPutResponse> PutAsync(Stream stream, string filename);

        Task<S3ObjectPutResponse> PutAsync(byte[] byteArr, string filename);

        Task<S3ObjectDeleteResponse> DeleteAsync(string filename);

        Task<S3MultipleObjectDeleteResponse> DeleteMultipleAsync(DeleteMultipleObjectsParameters parameters);
    }
}