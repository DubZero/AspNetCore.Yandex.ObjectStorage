using System.Xml.Serialization;

namespace AspNetCore.Yandex.ObjectStorage.Multipart.Models
{
    /// <summary>
    /// https://cloud.yandex.ru/docs/storage/s3/api-ref/multipart/startupload
    /// </summary>
    [XmlRoot(ElementName = "InitiateMultipartUploadResult")]
    internal class InitiateMultipartUploadResult
    {
        [XmlElement(ElementName = "Bucket")]
        public string Bucket { get; set; } = null!;

        [XmlElement(ElementName = "Key")]
        public string Key { get; set; } = null!;

        [XmlElement(ElementName = "UploadId")]
        public string UploadId { get; set; } = null!;
    }
}