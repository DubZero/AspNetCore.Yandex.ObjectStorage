using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket;
using AspNetCore.Yandex.ObjectStorage.Object;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

namespace AspNetCore.Yandex.ObjectStorage
{
    public interface IYandexStorageService
    {
        IObjectService ObjectService { get; }

        IBucketService BucketService { get; }

        // Not Implemented
        //IMultipartUploadService MultipartUploadService { get; }

        /// <summary>
        /// Test connection to storage
        /// </summary>
        /// <returns>Retruns true if all credentials correct</returns>
        Task<S3ObjectGetResponse> TryConnectAsync();
        /// <summary>
        /// Test connection to storage
        /// </summary>
        /// <returns>Retruns true if all credentials correct</returns>
        Task<S3ObjectGetResponse> TryGetAsync(string filename);
    }
}