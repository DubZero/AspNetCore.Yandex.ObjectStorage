using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.Yandex.ObjectStorage.Object.Models
{
    /// <summary>
    /// https://cloud.yandex.ru/docs/storage/s3/api-ref/object/deletemultipleobjects#request-scheme
    /// </summary>
    [XmlRoot(ElementName = "DeleteResult", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class MultipleDeleteResult
    {
        [XmlElement(ElementName = "Deleted", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public List<Deleted> Deleted { get; set; }
        [XmlElement(ElementName = "Error", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public List<Error> Error { get; set; }
    }

    [XmlRoot(ElementName = "Deleted", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Deleted
    {
        [XmlElement(ElementName="Key")]
        public string Key { get; set; }
    }

    [XmlRoot(ElementName = "Error", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Error
    {
        [XmlElement(ElementName = "Key", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public string Key { get; set; }
        [XmlElement(ElementName = "Code", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public string Code { get; set; }
        [XmlElement(ElementName = "Message", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
        public string Message { get; set; }
    }
}