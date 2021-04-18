using AspNetCore.Yandex.ObjectStorage.Enums;

namespace AspNetCore.Yandex.ObjectStorage.RequestOptionsModels
{
    public class BucketCreateOptions
    {
        public string BucketName { get; set; }
        public ACLType ACLType { get; set; }
    }
}