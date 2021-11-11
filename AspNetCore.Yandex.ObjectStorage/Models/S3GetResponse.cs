using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3GetResponse : S3Response
    {
        public S3GetResponse(HttpResponseMessage response) : base(response)
        {
            if (response.IsSuccessStatusCode)
            {
                Result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            Error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}