# Object Upload


```csharp
var storageService = new YandexStorageService(options);

S3ObjectPutResponse response = await storageService.ObjectService.PutAsync(byteArr, fileName);
```