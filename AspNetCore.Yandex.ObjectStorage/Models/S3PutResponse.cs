using System.Net.Http;
using System.Threading.Tasks;
using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3PutResponse : BaseS3Response
    {
        private readonly string _url;

        public S3PutResponse(HttpResponseMessage response, string url) : base(response)
        {
            _url = url;
        }

        /// <summary>
        /// Reads result from request
        /// </summary>
        /// <returns>Returns object url if Result.Ok, error text if Result.Failed</returns>
        public async Task<Result<string>> ReadResultAsStringAsync()
        {
            return IsSuccessStatusCode
                ? Result.Ok(_url)
                : Result.Fail(await ReadErrorAsync());
        }
    }
}