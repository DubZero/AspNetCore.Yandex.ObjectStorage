# AspNetCore.Yandex.ObjectStorage

.Net Core library for Yandex Object Storage S3 API (https://cloud.yandex.ru/docs/storage/s3/api-ref/).

[![Build Status](https://travis-ci.com/DubZero/AspNetCore.Yandex.ObjectStorage.svg?branch=master)](https://travis-ci.com/DubZero/AspNetCore.Yandex.ObjectStorage)


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
