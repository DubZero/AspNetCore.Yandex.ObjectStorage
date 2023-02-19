using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

using AspNetCore.Yandex.ObjectStorage.Bucket.Models;
using AspNetCore.Yandex.ObjectStorage.Models;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Multipart.Responses
{
    public class S3InitiateMultipartUploadResult : BaseS3Response
    {
        public S3InitiateMultipartUploadResult(HttpResponseMessage response) : base(response)
        {
        }

        public async Task<Result<InitiateMultipartUploadResult>> ReadResultAsync()
        {
            if (IsSuccessStatusCode)
            {
                var xmlSerializer = new XmlSerializer(typeof(BucketListResult));
                await using var stream = await Response.Content.ReadAsStreamAsync();

                if (!(xmlSerializer.Deserialize(stream) is InitiateMultipartUploadResult result))
                {
                    throw new InvalidDataException($"`{nameof(result)}` can't cast to {nameof(InitiateMultipartUploadResult)}");
                }

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