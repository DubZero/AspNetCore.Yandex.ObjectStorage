using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AspNetCore.Yandex.ObjectStorage.Bucket.Models;
using AspNetCore.Yandex.ObjectStorage.Models;
using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Responses
{
    public class S3BucketListResponse : BaseS3Response
    {
        public S3BucketListResponse(HttpResponseMessage response) : base(response)
        {
        }

        public async Task<Result<BucketListResult>> ReadResultAsync()
        {
            if (IsSuccessStatusCode)
            {
                var xmlSerializer = new XmlSerializer(typeof(BucketListResult));
                await using var stream = await Response.Content.ReadAsStreamAsync();

                var result = xmlSerializer.Deserialize(stream) as BucketListResult;

                return Result.Ok(result);
            }

            return Result.Fail(await Response.Content.ReadAsStringAsync());

        }

        public async Task<Result<string>> ReadResultAsStringAsync()
        {
            if (IsSuccessStatusCode)
            {
                return await Response.Content.ReadAsStringAsync();
            }

            return Result.Fail(await Response.Content.ReadAsStringAsync());
        }
    }
}