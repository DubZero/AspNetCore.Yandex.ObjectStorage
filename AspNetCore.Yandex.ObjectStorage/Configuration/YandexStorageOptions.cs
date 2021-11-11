using Microsoft.Extensions.Configuration;

namespace AspNetCore.Yandex.ObjectStorage.Configuration
{
	public class YandexStorageOptions
	{
		public YandexStorageOptions()
		{
		}

		public YandexStorageOptions(IConfigurationSection section)
		{
			BucketName = section.GetSection("Bucket").Value;
			AccessKey = section.GetSection("AccessKey").Value;
			SecretKey = section.GetSection("SecretKey").Value;

			Protocol = section.GetSection("Protocol")?.Value ?? YandexStorageDefaults.Protocol;
			Location = section.GetSection("Location")?.Value ?? YandexStorageDefaults.Location;
			Endpoint = section.GetSection("Endpoint")?.Value ?? YandexStorageDefaults.EndPoint;
		}

		/// <summary>
		/// "http" or "https"
		/// </summary>
		public string Protocol { get; set; } = YandexStorageDefaults.Protocol;

		/// <summary>
		/// https://cloud.yandex.ru/docs/storage/concepts/bucket
		/// </summary>
		public string BucketName { get; set; }

		public string Location { get; set; } = YandexStorageDefaults.Location;
		public string Endpoint { get; set; } = YandexStorageDefaults.EndPoint;

		/// <summary>
		/// https://cloud.yandex.ru/docs/storage/s3/
		/// </summary>
		public string AccessKey { get; set; }

		/// <summary>
		/// https://cloud.yandex.ru/docs/storage/s3/
		/// </summary>
		public string SecretKey { get; set; }

		public string HostName => $"{Protocol}://{Endpoint}/{BucketName}";
	}
}