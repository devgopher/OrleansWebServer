using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using OrleansWebServer.Backend.Grains.GrainsPool;
using OrleansWebServer.Backend.Grains.Interfaces;
using OrleansWebServer.Backend.Settings;

namespace OrleansWebServer.Backend.Extensions
{
    public static class ServiceExtensions
    {
        private const int GrainPoolsInitialCapacity = 30;

        public static void AddOrleansWebServerForGrains(this IServiceCollection services, IConfiguration config)
        {
            var serverConfig = new WebServerBackendSettings(); 
            config.GetSection(nameof(WebServerBackendSettings)).Bind(serverConfig);
            services.AddScoped<AsyncLogging.AsyncLogging, AsyncLogging.NLogLogger>();
            services.AddSingleton(serverConfig);
            services.AddScoped<WebServerBackend>();
        }

        public static void AddOrleansWebServerForRequest<TIn, TOut, TGrain>(this IServiceCollection services, IConfiguration config)
            where TGrain : class, IWebServerBackendGrain<TIn, TOut>, IGrainWithGuidKey
        {
            services.AddOrleansWebServerForGrains(config);
            //services.AddScoped(typeof(TGrain));
            var provider = services.BuildServiceProvider();
            var webServerBackend = provider.GetRequiredService<WebServerBackend>();
            webServerBackend.RegisterRequest<TGrain, TIn, TOut>();
        }

        public static void UseOrleansWebServerForGrains(this IApplicationBuilder app)
        {
        }
    }
}
