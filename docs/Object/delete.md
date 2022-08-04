# Object Delete


```csharp
var storageService = new YandexStorageService(options);

S3ObjectDeleteResponse response = await storageService.ObjectService.DeleteAsync(filename);
```