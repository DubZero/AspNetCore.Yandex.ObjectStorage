ApiKey=$1
Source=$2

dotnet nuget push ./bin/Release/AspNetCore.Yandex.ObjectStorage.*.nupkg -ApiKey $ApiKey -Source $Source