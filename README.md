# AspNetCore.Yandex.ObjectStorage

.Net Core library for Yandex Object Storage S3 API (https://cloud.yandex.ru/docs/storage/s3/api-ref).

[Nuget Package - AspNetCore.Yandex.ObjectStorage](https://www.nuget.org/packages/AspNetCore.Yandex.ObjectStorage/)

### How to use

To inject service user extension method `AddYandexObjectStorage` to `IServiceCollection`

```csharp
services.AddYandexObjectStorage(options =>
{
  options.BucketName = "bucketName";
  options.AccessKey = "your-access-key";
  options.SecretKey = "your-secret-key";
});
```

#### OR

Can load options from `IConfiguratiuonRoot` as: `services.AddYandexObjectStorage(Configuration);`
by default, it reads a section with the name `YandexObjectStorage`, for example, the section in `appsettings.json` below:
```json
"YandexObjectStorage":
{
    "Bucket" : "your-bucket",
    "AccessKey" : "your-access-key",
    "SecretKey" : "your-secret-key",
    "Protocol" : "https",
    "Location" : "us-east-1",
    "UseHttp2" : true
}
```

Options is a `YandexStorageOptions` class.
It provides access to setup next properties:
```
string Protocol - by default -> "https"
string BucketName
string Location - by default -> "us-east-1"
string Endpoint - by default -> "storage.yandexcloud.net"
string AccessKey
string SecretKey
boolean UseHttp2 - by default -> true
```

## Usage examples

```csharp
S3ObjectPutResponse response = await _objectStoreService.ObjectService.PutAsync(byteArr, fileName);
S3ObjectDeleteResponse response = await _objectStoreService.ObjectService.DeleteAsync(filename);
```

Get can return as Stream or ByteArray

```csharp
// result is FluentResults wrapped content of result
var result = await _objectStoreService.ObjectService.GetAsync(fileName);
if(result.IsSuccess) 
{
   byte[] byteArr = await result.ReadAsByteArrayAsync();
   Stream stream = await result.ReadAsStreamAsync();
}

if(result.IsFailed)
{
   var error = await result.ReadErrorAsync();
}
```

## List of implemented API methods

### Object service
| Method                                                        | Description                                                         | Status            |
|---------------------------------------------------------------|---------------------------------------------------------------------|-------------------|
| [upload](docs/Object/upload.md)                               | Uploads an object to Object Storage                                 | ✅ implemented     |
| [get](docs/Object/get.md)                                     | Retrieves an object from Object Storage                             | ✅ implemented     |
| [delete](docs/Object/delete.md)                               | Deletes an object                                                   | ✅ implemented     |
| [deleteMultipleObjects](docs/Object/deleteMultipleObjects.md) | Deletes objects based on a list                                     | ✅ implemented     |
| [options](docs/Object/options.md)                             | Checks whether a CORS request to an object can be made              | ✅ implemented     |
| [selectObjectContent](docs/Object/selectObjectContent.md)     | Filters and returns the contents of an object based on an SQL query | ✅ implemented     |
| [copy](docs/Object/copy.md)                                   | Copies an object stored in Object Storage                           | ❌ not implemented |
| [getObjectMeta](docs/Object/getObjectMeta.md)                 | Retrieves object metadata                                           | ❌ not implemented |

### Bucket Service 
| Method                                                       | Description                                                                                                         | Status            |
|--------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|-------------------|
| [create](docs/Bucket/create)                                 | Creates a bucket                                                                                                    | ✅ implemented     |
| [getMeta](docs/Bucket/getMeta)                               | Returns the bucket's metadata or an error                                                                           | ✅ implemented     |
| [listObjects](docs/Bucket/listObjects)                       | Returns a list of bucket objects. Pagination is used for output                                                     | ✅ implemented     |
| [listBuckets](docs/Bucket/listBuckets)                       | Returns a list of buckets available to the user                                                                     | ✅ implemented     |
| [deleteBucket](docs/Bucket/deleteBucket)                     | Deletes an empty bucket. If the bucket isn't empty, first delete all the objects inside the bucket                  | ✅ implemented     |
| [getBucketEncryption](docs/Bucket/getBucketEncryption)       | Returns information about bucket encryption. For more information about bucket encryption                           | ❌ not implemented |
| [putBucketEncryption](docs/Bucket/putBucketEncryption)       | Adds encryption to the bucket. By default, the objects added to the bucket are encrypted with the specified KMS key | ❌ not implemented |
| [deleteBucketEncryption](docs/Bucket/deleteBucketEncryption) | Removes encryption from the bucket. For more information about bucket encryption                                    | ❌ not implemented |
| [putBucketVersioning](docs/Bucket/putBucketVersioning)       | Enables or pauses versioning of the bucket                                                                          | ❌ not implemented |
| [getBucketVersioning](docs/Bucket/getBucketVersioning)       | Returns the versioning status                                                                                       | ❌ not implemented |
| [putBucketLogging](docs/Bucket/putBucketLogging)             | Enables and disables logging of actions with the bucket                                                             | ❌ not implemented |
| [getBucketLogging](docs/Bucket/getBucketLogging)             | Returns settings for logging actions with the bucket                                                                | ❌ not implemented |
| [listObjectVersions](docs/Bucket/listObjectVersions)         | Returns metadata for all versions of objects in the bucket                                                          | ❌ not implemented |

### Multipart upload service

| Method         | Description                          | Status            |
|----------------|--------------------------------------|-------------------|
| startUpload    | Starts multipart upload              | ❌ not implemented |
| uploadPart     | Uploads a part of an object          | ❌ not implemented |
| copyPart       | Copies part of an object             | ❌ not implemented |
| listParts      | Displays a list of uploaded parts    | ❌ not implemented |
| abortUpload    | Aborts multipart upload              | ❌ not implemented |
| completeUpload | Completes multipart upload           | ❌ not implemented |
| listUploads    | Returns a list of incomplete uploads | ❌ not implemented |


### Static Website Hosting service ❌ - not implemented
### CORS service ❌ - not implemented
### Lifecycles service ❌ - not implemented
### ACL service ❌ - not implemented
### Bucket Policy service ❌ - not implemented