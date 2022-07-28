using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage.Object;

namespace AspNetCore.Yandex.ObjectStorage
{
    public interface IYandexStorageService
    {
        IObjectService ObjectService { get; }

        // Not Implemented
        //IBucketService BucketService { get; }

        // Not Implemented
        //IMultipartUploadService MultipartUploadService { get; }

        /// <summary>
        /// Test connection to storage
        /// </summary>
        /// <returns>Retruns true if all credentials correct</returns>
        Task<S3GetResponse> TryConnectAsync();
        /// <summary>
        /// Test connection to storage
        /// </summary>
        /// <returns>Retruns true if all credentials correct</returns>
        Task<S3GetResponse> TryGetAsync(string filename);
    }
}