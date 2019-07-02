namespace AspNetCore.Yandex.ObjectStorage
{
    public class YandexStorageOptions
    {
        public string Protocol { get; } = YandexStorageDefaults.Protocol;
        public string BucketName { get; }
        public string Location { get; } = YandexStorageDefaults.Location;
        public string Endpoint { get; } = YandexStorageDefaults.EndPoint;
        public string AccessKey { get; }
        public string SecretKey { get; }

        public string HostName => $"{Protocol}://{Endpoint}/{BucketName}";
    }
}