namespace AspNetCore.Yandex.ObjectStorage
{
    public class YandexStorageOptions
    {
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
        public string AccessKey { get; set;}
        
        /// <summary>
        /// https://cloud.yandex.ru/docs/storage/s3/
        /// </summary>
        public string SecretKey { get; set; }
        public string HostName => $"{Protocol}://{Endpoint}/{BucketName}";
    }
}