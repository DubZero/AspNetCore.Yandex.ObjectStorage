using System.Net.Http;
using System.Threading.Tasks;
using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3DeleteResponse : BaseS3Response
    {

        public S3DeleteResponse(HttpResponseMessage response) : base(response)
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