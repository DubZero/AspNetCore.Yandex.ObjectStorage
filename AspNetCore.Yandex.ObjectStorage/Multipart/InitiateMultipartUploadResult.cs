using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.Yandex.ObjectStorage.Multipart
{
	public class InitiateMultipartUploadResult
	{
		public string Bucket { get; set; }

		public string Key { get; set; }

		public string UploadId { get; set; }
	}
}