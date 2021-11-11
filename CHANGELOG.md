<!-- Keep a Changelog guide -> https://keepachangelog.com -->

# AspNetCore.Yandex.ObjectStorage changelog

##[1.8.1]
### Changes
- Some namespaces changed due to directory nesting
- **Update .net version to 6.0**
- Using `Location` from options
- Add some directories to structure code
- Add more documentation
### Fixed
- Removes warning messages from not implemented methods

##[1.7.6]
- HttpClient now is static readonly
- HttpCompletionOption.ResponseHeadersRead option for GetAsStreamAsync and TryGetAsync
- Migrate to .net 5.0