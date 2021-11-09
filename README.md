# AspNetCore.Yandex.ObjectStorage

.Net Core library for Yandex Object Storage S3 API (https://cloud.yandex.ru/docs/storage/s3/api-ref/).

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
