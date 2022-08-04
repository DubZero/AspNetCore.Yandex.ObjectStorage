using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Models;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Object.Responses
{
    public class S3ObjectGetResponse : BaseS3Response
    {
        public S3ObjectGetResponse(HttpResponseMessage response) : base(response)
        {
        }

        public async Task<Result<Stream>> ReadAsStreamAsync()
        {
            if (IsSuccessStatusCode)
            {
                return await Response.Content.ReadAsStreamAsync();
            }

            return Result.Fail(await Response.Content.ReadAsStringAsync());

        }

        public async Task<Result<byte[]>> ReadAsByteArrayAsync()
        {
            if (IsSuccessStatusCode)
            {
                return await Response.Content.ReadAsByteArrayAsync();
            }

            return Result.Fail(await Response.Content.ReadAsStringAsync());
        }
    }
}