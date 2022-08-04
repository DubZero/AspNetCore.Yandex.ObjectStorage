# Object Get

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