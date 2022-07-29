namespace AspNetCore.Yandex.ObjectStorage.Bucket.Requests
{
    public class BucketListObjectsParameters
    {
        public string BucketName { get; set; }

        public int MaxKeys { get; set; } = 1000;

        public string Delimiter { get; set; }

        public string Prefix { get; set; }

        public string StartAfter { get; set; }

        public string ContinueToken { get; set; }
    }
}