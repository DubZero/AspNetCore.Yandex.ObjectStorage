namespace AspNetCore.Yandex.ObjectStorage
{
    public class YandexStorageOptions
    {
        public string Protocol { get; set; } = YandexStorageDefaults.Protocol;
        public string BucketName { get; set; }
        public string Location { get; set; } = YandexStorageDefaults.Location;
        public string Endpoint { get; set; } = YandexStorageDefaults.EndPoint;
        public string AccessKey { get; set;}
        public string SecretKey { get; set; }
        public string HostName => $"{Protocol}://{Endpoint}/{BucketName}";
    }
}