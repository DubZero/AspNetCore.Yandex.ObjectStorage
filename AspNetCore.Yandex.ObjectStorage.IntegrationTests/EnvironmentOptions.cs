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
                BucketName = "testbucketlib",
                AccessKey = "YCAJEmcixPRabFaAWHp-k_RA7",
                SecretKey = "YCOflzMKw_FF5GjpxX2jMjSz7fgfj7cT_aItejSz"
            };
        }

        public static YandexStorageOptions GetFromEnvironmentWithNotDefaultLocation()
        {
            return new YandexStorageOptions
            {
                BucketName = "testbucketlib",
                AccessKey = "YCAJEmcixPRabFaAWHp-k_RA7",
                SecretKey = "YCOflzMKw_FF5GjpxX2jMjSz7fgfj7cT_aItejSz",
                Location = "ru-central1",
                Endpoint = "s3.yandexcloud.net"
            };
        }
    }
}