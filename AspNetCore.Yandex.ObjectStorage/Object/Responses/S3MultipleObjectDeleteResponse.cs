using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

using AspNetCore.Yandex.ObjectStorage.Object.Models;

using FluentResults;

namespace AspNetCore.Yandex.ObjectStorage.Object.Responses
{
    public class S3MultipleObjectDeleteResponse : S3ObjectDeleteResponse
    {
        public S3MultipleObjectDeleteResponse(HttpResponseMessage response) : base(response)
        {
        }

        public async Task<Result<MultipleDeleteResult>> ReadResultAsync()
        {
            if (IsSuccessStatusCode)
            {
                var xmlSerializer = new XmlSerializer(typeof(MultipleDeleteResult));
                await using var stream = await Response.Content.ReadAsStreamAsync();

                var result = xmlSerializer.Deserialize(stream) as MultipleDeleteResult;

                return Result.Ok(result);
            }

            return Result.Fail(await Response.Content.ReadAsStringAsync());
        }
    }
}