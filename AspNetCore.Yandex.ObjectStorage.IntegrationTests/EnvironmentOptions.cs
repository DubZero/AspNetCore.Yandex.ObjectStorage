using System;
using AspNetCore.Yandex.ObjectStorage.Configuration;

namespace AspNetCore.Yandex.ObjectStorage.IntegrationTests
{
    public static class EnvironmentOptions
    {
        public static YandexStorageOptions GetFromEnvironment()
        {
            return new YandexStorageOptions
            {
                BucketName = Environment.GetEnvironmentVariable("BucketName"),
                AccessKey = Environment.GetEnvironmentVariable("AccessKey"),
                SecretKey = Environment.GetEnvironmentVariable("SecretKey")
            };
        }
    }
}