ApiKey=$1
Source=$2

dotnet pack ./obj/Release/AspNetCore.Yandex.ObjectStorage.nuspec -Verbosity detailed

dotnet nuget push ./bin/Release/AspNetCore.Yandex.ObjectStorage.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
