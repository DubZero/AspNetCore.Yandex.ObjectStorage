using System;
using System.Net.Http;
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

    public class BucketService : IBucketService
    {
        public async Task<S3PutResponse> CreateBucket(string bucketName)
        {
            var builder = new BucketPutRequestBuilder(_options);
            var createModel = new BucketCreateOptions()
            {
                BucketName = bucketName,
                ACLType = ACLType.Private
            };
            var requestMessage = builder.Build(createModel).GetResult();

            using var client = new HttpClient();
            var response = await client.SendAsync(requestMessage);

            return new S3PutResponse(response, GetBucketUri(bucketName));
        }

        public Task<S3GetResponse> GetBucketMeta()
        {
            throw new NotImplementedException();
        }

        public Task<S3GetResponse> GetBucketListObjects()
        {
            throw new NotImplementedException();
        }

        public Task<S3GetResponse> GetBucketList()
        {
            throw new NotImplementedException();
        }

        public Task<S3DeleteResponse> DeleteBucket()
        {
            throw new NotImplementedException();
        }
    }
}