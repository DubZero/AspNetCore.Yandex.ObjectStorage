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
            var readedOptions = configuration.GetYandexStorageOptions(sectionName);

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