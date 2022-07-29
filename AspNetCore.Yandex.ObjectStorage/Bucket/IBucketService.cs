using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Bucket.Requests;
using AspNetCore.Yandex.ObjectStorage.Bucket.Responses;
using AspNetCore.Yandex.ObjectStorage.Models;

namespace AspNetCore.Yandex.ObjectStorage.Bucket
{
    public interface IBucketService
    {
        Task<S3PutResponse> CreateBucket(string bucketName);

        Task<S3GetResponse> GetBucketMeta(string bucketName);

        Task<S3BucketObjectListResponse> GetBucketListObjects(BucketListObjectsParameters parameters);

        Task<S3BucketListResponse> GetBucketList();

        Task<S3DeleteResponse> DeleteBucket(string bucketName);
    }
}