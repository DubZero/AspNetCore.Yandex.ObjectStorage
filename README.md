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
S3PutResponse response = await _objectStoreService.PutObjectAsync(byteArr, fileName);
S3DeleteResponse response = await _objectStoreService.DeleteObjectAsync(filename);
```
Get can return as Stream or ByteArray

```
byte[] byteArr = await _objectStoreService.GetAsByteArrayAsync(fileName);
Stream stream = await _objectStoreService.GetAsStreamAsync(filename);
```

## List of implemented API methods

### Bucket Service ❌ - not implemented
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
