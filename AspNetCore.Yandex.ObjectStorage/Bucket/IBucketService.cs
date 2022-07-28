using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Models;

namespace AspNetCore.Yandex.ObjectStorage.Bucket
{
    public interface IBucketService
    {
        Task<S3PutResponse> CreateBucket(string bucketName);

        Task<S3GetResponse> GetBucketMeta();

        Task<S3GetResponse> GetBucketListObjects();

        Task<S3GetResponse> GetBucketList();

        Task<S3DeleteResponse> DeleteBucket();
    }
}