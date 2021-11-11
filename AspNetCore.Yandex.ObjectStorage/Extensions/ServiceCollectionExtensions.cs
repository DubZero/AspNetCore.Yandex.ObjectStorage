using System;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddYandexObjectStorage(this IServiceCollection services, Action<YandexStorageOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            services.Configure(setupAction);
            services.AddTransient<YandexStorageService>();
        }

        public static void AddYandexObjectStorage(this IServiceCollection services, IConfiguration configuration, string sectionName = YandexConfigurationDefaults.DefaultSectionName)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.LoadYandexStorageOptions(configuration, sectionName).AddTransient<YandexStorageService>();
        }

        public static YandexStorageService CreateYandexObjectService(this YandexStorageOptions options)
        {
            return new YandexStorageService(options);
        }
    }
}