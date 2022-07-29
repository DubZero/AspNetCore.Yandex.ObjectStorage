# AspNetCore.Yandex.ObjectStorage

.Net Core library for Yandex Object Storage S3 API (https://cloud.yandex.ru/docs/storage/s3/api-ref).

[Nuget Package - AspNetCore.Yandex.ObjectStorage](https://www.nuget.org/packages/AspNetCore.Yandex.ObjectStorage/)

### How to use

To inject service user extension method `AddYandexObjectStorage` to `IServiceCollection`

```
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
```
"YandexObjectStorage" : {
    "Bucket" : "your-bucket",
    "AccessKey" : "your-access-key",
    "SecretKey" : "your-secret-key",
    "Protocol" : "http"
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
```

## Usage examples

```
S3PutResponse response = await _objectStoreService.ObjectService.PutAsync(byteArr, fileName);
S3DeleteResponse response = await _objectStoreService.ObjectService.DeleteAsync(filename);
```
Get can return as Stream or ByteArray

```
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

### Bucket Service 
- create - Creates a bucket ✅ implemented
- getMeta - Returns the bucket's metadata or an error ✅ implemented
- listObjects - Returns a list of bucket objects. Pagination is used for output ✅ implemented
- listBuckets - Returns a list of buckets available to the user ✅ implemented
- deleteBucket - Deletes an empty bucket. If the bucket isn't empty, first delete all the objects inside the bucket ✅ implemented
- getBucketEncryption - ❌ not implemented
- deleteBucketEncryption - ❌ not implemented
- putBucketEncryption - ❌ not implemented
- putBucketVersioning - ❌ not implemented
- getBucketVersioning - ❌ not implemented
- putBucketLogging - ❌ not implemented
- getBucketLogging - ❌ not implemented
- listObjectVersions - ❌ not implemented

### Object service
- upload	Uploads an object to Object Storage. ✅ implemented
- get	Retrieves an object from Object Storage. ✅ implemented
- copy	Copies an object stored in Object Storage. ✅ implemented
- getObjectMeta	Retrieves object metadata. ❌ not implemented
- delete	Deletes an object. ✅ implemented
- deleteMultipleObjects	Deletes objects based on a list. ❌ not implemented
- options	Checks whether a CORS request to an object can be made. ❌ not implemented
- selectObjectContent	Filters and returns the contents of an object based on an SQL query. ❌ not implemented
### Multipart upload service
- startUpload	Starts multipart upload. ❌ prototype
- uploadPart	Uploads a part of an object. ❌ prototype
- copyPart	Copies part of an object. ❌ prototype
- listParts	Displays a list of uploaded parts. ❌ prototype
- abortUpload	Aborts multipart upload. ❌ prototype
- completeUpload	Completes multipart upload. ❌ prototype
- listUploads	Returns a list of incomplete uploads. ❌ prototype
### Static Website Hosting service ❌ - not implemented
### CORS service ❌ - not implemented
### Lifecycles service ❌ - not implemented
### ACL service ❌ - not implemented
### Bucket Policy service ❌ - not implemented
