using System.Net;
using System.Net.Http;

namespace AspNetCore.Yandex.ObjectStorage.Models
{
	public abstract class S3Response
	{
		protected readonly HttpResponseMessage _response;

		protected S3Response(HttpResponseMessage response)
		{
			_response = response;
		}

		public bool IsSuccessStatusCode => _response.IsSuccessStatusCode;
		public HttpStatusCode StatusCode => _response.StatusCode;

		public string Error { get; protected set; }

		public string Result { get; protected set; }
	}
}