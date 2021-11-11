using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public class S3DeleteResponse : S3Response
    {
        public S3DeleteResponse(HttpResponseMessage response) : base(response)
        {
            if (response.IsSuccessStatusCode)
            {
                Result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }

            Error = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public bool IsSuccess => IsSuccessStatusCode;
    }
}