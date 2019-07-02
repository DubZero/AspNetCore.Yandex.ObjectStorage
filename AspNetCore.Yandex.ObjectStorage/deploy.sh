ApiKey=$1
Source=$2

nuget pack ./obj/Release/AspNetCore.Yandex.ObjectStorage.nuspec -Verbosity detailed

nuget push ./bin/Release/AspNetCore.Yandex.ObjectStorage.*.nupkg -Verbosity detailed -ApiKey $ApiKey -Source $Source
