using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AspNetCore.Yandex.ObjectStorage.Bucket.Models
{
    /// <summary>
    /// https://cloud.yandex.ru/docs/storage/s3/api-ref/bucket/listobjects#structureV2
    /// </summary>
    [XmlRoot(ElementName="ListBucketResult", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ObjectListResult
    {
        /// <summary>
        /// Флаг, показывающий все ли результаты возвращены в этом ответе.
        /// </summary>
        [XmlElement(ElementName="IsTruncated")]
        public bool IsTruncated { get; set; }

        /// <summary>
        /// Описание объекта
        /// </summary>
        [XmlElement(ElementName="Contents", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
        public List<Contents> Contents { get; set; }

        /// <summary>
        /// Имя бакета.
        /// </summary>
        [XmlElement(ElementName="Name")]
        public string Name { get; set; }

        /// <summary>
        /// Значение query-параметра delimiter
        /// </summary>
        [XmlElement(ElementName="Delimiter")]
        public string Delimiter { get; set; }

        /// <summary>
        /// Значение query-параметра max-keys
        /// </summary>
        [XmlElement(ElementName="MaxKeys")]
        public int MaxKeys { get; set; }

        /// <summary>
        /// Часть имени ключа, которая определяется при обработке query-параметров delimiter и prefix
        /// </summary>
        [XmlElement(ElementName="CommonPrefixes")]
        public CommonPrefixes CommonPrefixes { get; set; }

        /// <summary>
        /// Кодировка, в которой Object Storage представляет ключ в XML-ответе.
        /// </summary>
        [XmlElement(ElementName="EncodingType")]
        public string EncodingType { get; set; }

        /// <summary>
        /// Количество ключей, возвращенных запросом.
        /// Количество ключей всегда будет меньше или равно MaxKeys
        /// </summary>
        [XmlElement(ElementName="KeyCount")]
        public int KeyCount { get; set; }

        /// <summary>
        /// Значение query-параметра continuation-token.
        /// </summary>
        [XmlElement(ElementName="ContinuationToken")]
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Значение, которое надо подставить в query-параметр continuation-token для получения следующей части списка, если весь список не поместился в текущий ответ.
        /// возвращается только в том случае, если IsTruncated = true
        /// </summary>
        [XmlElement(ElementName="NextContinuationToken")]
        public string NextContinuationToken { get; set; }

        /// <summary>
        /// Значение query-параметра start-after
        /// </summary>
        [XmlElement(ElementName="StartAfter")]
        public string StartAfter { get; set; }
    }


    [XmlRoot(ElementName="CommonPrefixes")]
    public class CommonPrefixes
    {
        public string Prefix { get; set; }
    }

    [XmlRoot(ElementName="Contents", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Contents
    {
        /// <summary>
        /// MD5-хэш объекта. Метаданные в расчете хэша не участвуют.
        /// </summary>
        public string ETag { get; set; }
        /// <summary>
        /// Ключ объекта.
        /// </summary>
        [XmlElement(ElementName="Key")]
        public string Filename { get; set; }
        /// <summary>
        /// Дата и время последнего изменения объекта.
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Размер объекта в байтах.
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Класс хранения объекта: STANDARD или COLD.
        /// </summary>
        public string StorageClass { get; set; }

        public Owner Owner { get; set; }
    }

    [XmlRoot(ElementName="Owner", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Owner
    {
        [XmlElement(ElementName="ID", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
        public string Id { get; set; }
        [XmlElement(ElementName="DisplayName", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
        public string DisplayName { get; set; }
    }
}