using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.Yandex.ObjectStorage.Object.Parameters
{
    /// <summary>
    /// https://cloud.yandex.com/en-ru/docs/storage/s3/api-ref/object/deletemultipleobjects#request-scheme
    /// </summary>
    [XmlRoot(ElementName = "Delete")]
    public class DeleteMultipleObjectsParameters
    {
        /// <summary>
        /// Object Storage only includes deletion errors in the response. If there are no errors, there won't be a response body.
        /// </summary>
        [XmlElement(ElementName = "Quiet")]
        public bool IsQuite { get; set; }

        [XmlElement(ElementName = "Object")]
        public List<DeleteObject> DeleteObjects { get; set; }

    }

    [XmlRoot(ElementName = "Object")]
    public class DeleteObject
    {
        [XmlElement(ElementName = "Key")]
        public string Key { get; set; }
    }
}