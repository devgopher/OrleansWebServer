using AsyncLogging;
using McGuireV10.OrleansDistributedCache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using OWS.Grains.RemoteExecutionAssemblies;
using OWS.Models.Settings;
using OWS.SiloHost.Utils;
using OWSUtils;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OWS
{
    public class Program
    {
        private static IAsyncLogger _logger;

        public static Task Main(string[] args)
        {
            var oskSettings = new OskSettings();
            var siloSettings = new SiloSettings();
            _logger = new NLogLogger();

            _logger.Info("Starting SiloHost...");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection("OskSettings").Bind(oskSettings);
            configuration.GetSection("SiloSettings").Bind(siloSettings);

            if (siloSettings.MaxCpuLoad < 100)
                Task.Run(() => CpuOptimizer.Start(siloSettings.MaxCpuLoad, new CancellationToken()));

            var dir = Directory.GetCurrentDirectory();

            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(oskSettings);
                            services.AddSingleton(siloSettings);
                            services.AddScoped(l => _logger);
                            services.AddSingleton<IAssemblyCache, MemoryAssemblyCache>();
                            services.AddSingleton<IAssemblyMembersCache, MemoryAssemblyMembersCache>();
                            if (siloSettings.Caching == true)
                                services.AddOrleansDistributedCache(opt =>
                                {
                                    opt.DefaultDelayDeactivation = TimeSpan.FromMinutes(5);
                                    opt.PersistWhenSet = true;
                                });
                        })
                        .Configure((Action<SchedulingOptions>)(options => options.AllowCallChainReentrancy = false))
                        .Configure((Action<ClusterOptions>)(options =>
                        {
                            options.ClusterId = oskSettings.ClusterId;
                            options.ServiceId = oskSettings.ServiceId;
                        }))
                        .Configure((Action<EndpointOptions>)(options => options.AdvertisedIPAddress = IpUtils.IpAddress()))
                        .ConfigureApplicationParts(parts => AsmLoaderUtils.AddParts(_logger, parts, siloSettings))
                        .UseDashboard(options => {
                            options.Host = "*";
                            options.Port = 8080;
                            options.HostSelf = true;
                            options.CounterUpdateIntervalMs = 10000;
                        })
                        .AddMemoryGrainStorage(name: "OWSStorage")
                        .AddSimpleMessageStreamProvider("OSKProvider", c => c.OptimizeForImmutableData = true);                    
                })
                .ConfigureLogging(builder => builder.AddConsole())
                .RunConsoleAsync();
        }
    }
}
