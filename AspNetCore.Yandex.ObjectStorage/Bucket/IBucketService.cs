using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Bucket.Responses;
using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage.Object.Responses;

namespace AspNetCore.Yandex.ObjectStorage.Bucket
{
    public interface IBucketService
    {
        Task<S3ObjectPutResponse> CreateBucket(string bucketName);

        /// <summary>
        /// Returns the bucket's metadata or an error.
        /// Use this method to check:
        ///   1. Whether the bucket exists.
        ///   2. Whether the user has sufficient permissions to access the bucket
        /// </summary>
        Task<S3Response> GetBucketMeta(string bucketName);

        /// <summary>
        /// Get list of objects in bucket
        /// </summary>
        Task<S3BucketObjectListResponse> GetBucketListObjects(BucketListObjectsParameters parameters);

        /// <summary>
        /// Get list of buckets
        /// </summary>
        Task<S3BucketListResponse> GetBucketList();

        Task<S3ObjectDeleteResponse> DeleteBucket(string bucketName);
    }
}