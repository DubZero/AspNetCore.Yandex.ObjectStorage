using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage.Configuration
{
    internal static class YandexConfigurationReaderExtension
    {
        private static YandexStorageOptions GetYandexStorageOptions(this IConfiguration configuration, string sectionName)
        {
            var section = configuration.GetSection(sectionName);

            return new YandexStorageOptions(section);
        }


        internal static IServiceCollection LoadYandexStorageOptions(this IServiceCollection services, IConfiguration configuration, string sectionName)
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
                options.UseHttp2 = readOptions.UseHttp2;
            });

            return services;
        }
    }
}