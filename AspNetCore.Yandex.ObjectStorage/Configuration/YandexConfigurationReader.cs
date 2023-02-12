using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage.Configuration
{
    public static class YandexConfigurationReaderExtension
    {
        public static YandexStorageOptions GetYandexStorageOptions(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);

            return new YandexStorageOptions(section);
        }


        public static IServiceCollection LoadYandexStorageOptions(this IServiceCollection services, IConfiguration configuration, string sectionName)
        {
            var readOptions = configuration.GetYandexStorageOptions(sectionName);

            new YandexStorageOptionsValidator().ValidateOrThrow(readOptions);

            services.Configure<YandexStorageOptions>(options =>
            {
                options.BucketName = readOptions.BucketName;
                options.Location = readOptions.Location;
                options.AccessKey = readOptions.AccessKey;
                options.SecretKey = readOptions.SecretKey;
                options.Endpoint = readOptions.Endpoint;
                options.Protocol = readOptions.Protocol;
            });

            return services;
        }
    }
}