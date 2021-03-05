using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Yandex.ObjectStorage
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

		public Task<string>  Error { get; protected set; }
		
		public string Result { get; protected set; }
	}
	
	public class S3PutResponse : S3Response
	{
		private string _url;
		public S3PutResponse(HttpResponseMessage response, string url) : base(response)
		{
			_url = url;
			if (response.IsSuccessStatusCode)
			{
				Result = url;
			}
			else
			{
				Error = response.Content.ReadAsStringAsync();
			}
		}
	}
	
	public class S3DeleteResponse : S3Response
	{
		public S3DeleteResponse(HttpResponseMessage response) : base(response)
		{
			if (response.IsSuccessStatusCode)
			{
				Result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
			}

			Error = response.Content.ReadAsStringAsync();
		}
	}
}