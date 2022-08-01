<!-- Keep a Changelog guide -> https://keepachangelog.com -->

# AspNetCore.Yandex.ObjectStorage changelog

## [0.2.1.0]
## What's Changed
- Added BucketService
> create - Creates a bucket ✅ implemented
> getMeta - Returns the bucket's metadata or an error ✅ implemented
> listObjects - Returns a list of bucket objects. Pagination is used for output ✅ implemented
> listBuckets - Returns a list of buckets available to the user ✅ implemented
> deleteBucket - Deletes an empty bucket. If the bucket isn't empty, first delete all the objects inside the bucket ✅ implemented

## [0.2.0.0]

## Changes
- Rework response models, remove GetAwaiter().GetResult() in ctor
- Result of response wrapped in [FluentResults](https://github.com/altmann/FluentResults)
- Add API segregation to storage service (ObjectService, BucketService, MultipartUploadService ...)
```csharp
// BEFOR
var result = await _yandexStorageService.PutObjectAsync.(fakeObject, filename);

// NOW
var result = await _yandexStorageService.ObjectService.PutAsync(fakeObject, filename);
```

## [0.1.8.4]
### Changes
- Going from .net core, .net to .net standart
- Add some tests to ci pipeline
### Fixed
- Location options, that not equal default not works. fixed.

## [0.1.8.1]
### Changes
- Some namespaces changed due to directory nesting
- **Update .net version to 6.0**
- Using `Location` from options
- Add some directories to structure code
- Add more documentation
### Fixed
- Removes warning messages from not implemented methods

## [0.1.7.6]
- HttpClient now is static readonly
- HttpCompletionOption.ResponseHeadersRead option for GetAsStreamAsync and TryGetAsync
- Migrate to .net 5.0
