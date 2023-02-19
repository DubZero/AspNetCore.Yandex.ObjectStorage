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
            BucketName = section.GetSection("Bucket").Value ?? string.Empty;
            AccessKey = section.GetSection("AccessKey").Value ?? string.Empty;
            SecretKey = section.GetSection("SecretKey").Value ?? string.Empty;

            Protocol = section.GetSection("Protocol").Value ?? YandexStorageDefaults.Protocol;
            Location = section.GetSection("Location").Value ?? YandexStorageDefaults.Location;
            Endpoint = section.GetSection("Endpoint").Value ?? YandexStorageDefaults.EndPoint;

            if(bool.TryParse(section.GetSection("UseHttp2").Value, out var useHttp2))
            {
                UseHttp2 = useHttp2;
            }
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

        /// <summary>
        /// Use http2, true by default
        /// </summary>
        public bool UseHttp2 { get; set; } = true;
    }
}