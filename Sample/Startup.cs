using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            // 1. Way one
            // services.AddYandexObjectStorage(cfg =>
            // {
            //     cfg.AccessKey = "your-bucket";
            //     cfg.BucketName = "your-access-key";
            //     cfg.SecretKey = "your-secret-key";
            // });

            // 2. Way two
            services.AddYandexObjectStorage(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, YandexStorageService service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Test injection
            service.TryConnectAsync().GetAwaiter().GetResult();
        }
    }
}