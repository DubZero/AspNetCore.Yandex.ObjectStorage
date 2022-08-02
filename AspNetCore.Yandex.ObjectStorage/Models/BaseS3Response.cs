using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
    public abstract class BaseS3Response
    {
        internal BaseS3Response(HttpResponseMessage response)
        {
            Response = response;
        }

        protected readonly HttpResponseMessage Response;

        public bool IsSuccessStatusCode => Response.IsSuccessStatusCode;
        public HttpStatusCode StatusCode => Response.StatusCode;

        protected virtual async Task<string> ReadErrorAsync()
        {
            if (IsSuccessStatusCode)
            {
                return string.Empty;
            }

            return await Response.Content.ReadAsStringAsync();
        }
    }
}