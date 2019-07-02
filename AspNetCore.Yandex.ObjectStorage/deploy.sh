ApiKey=$1
Source=$2

dotnet pack ./obj/Release/AspNetCore.Yandex.ObjectStorage.nuspec -v detailed

dotnet nuget push ./bin/Release/AspNetCore.Yandex.ObjectStorage.*.nupkg -v detailed -ApiKey $ApiKey -Source $Source
