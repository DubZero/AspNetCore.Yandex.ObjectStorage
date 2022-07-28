using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3GetResponse : BaseS3Response
    {
        public S3GetResponse(HttpResponseMessage response) : base(response)
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