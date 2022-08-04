using System.Net.Http;
using System.Threading.Tasks;

using AspNetCore.Yandex.ObjectStorage.Models;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Object.Responses
{
    public class S3ObjectPutResponse : BaseS3Response
    {
        private readonly string _url;

        public S3ObjectPutResponse(HttpResponseMessage response, string url) : base(response)
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