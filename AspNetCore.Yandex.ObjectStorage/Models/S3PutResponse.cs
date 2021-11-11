using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3PutResponse : S3Response
    {
        public S3PutResponse(HttpResponseMessage response, string url) : base(response)
        {
            if (response.IsSuccessStatusCode)
            {
                Result = url;
            }
            else
            {
                Error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }
    }
}