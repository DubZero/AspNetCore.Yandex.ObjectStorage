using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage
{
    public static class YandexConfigurationReaderExtension
    {
        public static YandexStorageOptions GetYandexStorageOptions(this IConfiguration configuration)
        {
            var section = configuration.GetSection(YandexConfigurationDefaults.DefaultSectionName);
            
            return new YandexStorageOptions(section);
        }
        
        public static IServiceCollection LoadYandexStorageOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var readedOptions = configuration.GetYandexStorageOptions();
            
            services.Configure<YandexStorageOptions>(options =>
            {
                options.BucketName = readedOptions.BucketName;
                options.Location = readedOptions.Location;
                options.AccessKey = readedOptions.AccessKey;
                options.SecretKey = readedOptions.SecretKey;
                options.Endpoint = readedOptions.Endpoint;
                options.Protocol = readedOptions.Protocol;
            });

            return services;
        }
    }
}