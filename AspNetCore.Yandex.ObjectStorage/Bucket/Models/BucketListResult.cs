using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Models
{
    /// <summary>
    /// https://cloud.yandex.ru/docs/storage/s3/api-ref/bucket/list#response-scheme
    /// </summary>
    [XmlRoot(ElementName = "ListAllMyBucketsResult", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class BucketListResult
    {
        [XmlArray("Buckets", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        [XmlArrayItem("Bucket", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public List<Bucket> Buckets { get; set; }
    }

    [XmlRoot(ElementName = "Bucket", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Bucket
    {
        [XmlElement(ElementName = "Name", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public string Name { get; set; }
        [XmlElement(ElementName = "CreationDate", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public string CreationDate { get; set; }
    }

}