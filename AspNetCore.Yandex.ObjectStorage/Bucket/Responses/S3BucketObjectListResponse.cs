using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

using AspNetCore.Yandex.ObjectStorage.Bucket.Models;
using AspNetCore.Yandex.ObjectStorage.Models;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Responses
{
    public class S3BucketObjectListResponse : BaseS3Response
    {
        public S3BucketObjectListResponse(HttpResponseMessage response) : base(response)
        {
        }

        public async Task<Result<ObjectListResult>> ReadResultAsync()
        {
            if (IsSuccessStatusCode)
            {
                var xmlSerializer = new XmlSerializer(typeof(ObjectListResult));
                await using var stream = await Response.Content.ReadAsStreamAsync();

                var result = xmlSerializer.Deserialize(stream) as ObjectListResult;

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