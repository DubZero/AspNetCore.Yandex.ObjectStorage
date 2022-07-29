using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Models;
using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Object.Models
{
    public class S3ObjectDeleteResponse : BaseS3Response
    {

        public S3ObjectDeleteResponse(HttpResponseMessage response) : base(response)
        {

        }

        public async Task<Result<string>> ReadResultAsStringAsync()
        {
            return IsSuccessStatusCode
                ? Result.Ok(await Response.Content.ReadAsStringAsync())
                : Result.Fail(await ReadErrorAsync());
        }
    }
}