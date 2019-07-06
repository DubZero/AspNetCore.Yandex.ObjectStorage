using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Yandex.ObjectStorage
{
    public static class YandexStorageExtension
    {
        public static void AddYandexObjectStorage(this IServiceCollection services, Action<YandexStorageOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            services.Configure(setupAction);
            services.AddTransient<YandexStorageService>();
        }
    }

}