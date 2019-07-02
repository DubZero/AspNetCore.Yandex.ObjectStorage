using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage
{
    public static class YandexStorageExtension
    {
        public static IServiceCollection AddYandexObjectStorage(this IServiceCollection services, YandexStorageOptions options)
        {
            return services.AddTransient(p => new YandexStorageService(options));
        }
    }

}