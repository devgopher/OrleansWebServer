using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;

namespace OrleansStatisticsKeeper.Client.Services.Utils
{
    public static class ServiceAddons
    {
        private static BackgroundJobServer _backgroundJobServer = default;
        public static void UseOskScheduler(this IServiceCollection services)
        {
            services.AddHangfire(cfg =>
            {
                cfg.UseSimpleAssemblyNameTypeSerializer()
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseRecommendedSerializerSettings()
                    .UseMemoryStorage();
            });

            services.AddHangfireServer();
        }

        public static void StopJobServer() => _backgroundJobServer?.SendStop();
    }
}
