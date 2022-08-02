using System.Net.Http;
using System.Threading.Tasks;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3Response : BaseS3Response
    {
        public S3Response(HttpResponseMessage response) : base(response)
        {
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