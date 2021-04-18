using System;
using AspNetCore.Yandex.ObjectStorage;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();

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
			var result = service.GetAsByteArrayAsync("teststorage.txt").GetAwaiter().GetResult();
			var rnd = new Random();
			
			var responce = service.CreateBucket($"testbucket{rnd.Next(1,500)}").GetAwaiter().GetResult();
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync(responce.Error);
			});
		}
	}
}