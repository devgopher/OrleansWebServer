using AsyncLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using OWS.Grains.Models;
using OWS.Grains.RemoteExecutionAssemblies;
using OWS.Models.Settings;
using OWS.SiloHost.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using OrleansWebServer.Grains;
using OWSUtils;
using System.Runtime.Loader;

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
                            services.AddScoped<IAsyncLogger>(l => _logger);
                            services.AddSingleton<IAssemblyCache, MemoryAssemblyCache>();
                            services.AddSingleton<IAssemblyMembersCache, MemoryAssemblyMembersCache>();
                        })
                        .Configure((Action<SchedulingOptions>)(options => options.AllowCallChainReentrancy = false))
                        .Configure((Action<ClusterOptions>)(options =>
                        {
                            options.ClusterId = oskSettings.ClusterId;
                            options.ServiceId = oskSettings.ServiceId;
                        }))
                        .Configure((Action<EndpointOptions>)(options => options.AdvertisedIPAddress = IpUtils.IpAddress()))
                        .ConfigureApplicationParts(parts => AddParts(parts, siloSettings))
                        .UseDashboard(options => {
                            options.Host = "*";
                            options.Port = 8080;
                            options.HostSelf = true;
                            options.CounterUpdateIntervalMs = 1000;
                        })
                        .AddMemoryGrainStorage(name: "StatisticsGrainStorage")
                        .AddSimpleMessageStreamProvider("OSKProvider", c => c.OptimizeForImmutableData = true);                    
                })
                .ConfigureLogging(builder => builder.AddConsole())
                .RunConsoleAsync();
        }

        private static Assembly[] GetLinkedAssemblies(SiloSettings siloSettings)
        {
            _logger.Info("Adding linked assmeblies for grains ...");
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            var basicDirectory = directoryInfo?.FullName;
            basicDirectory = directoryInfo != null ? directoryInfo.Parent?.Parent?.FullName : basicDirectory;
         
            var asmPaths = siloSettings.ModelsAssemblies?.SelectMany(x => Directory.GetFiles(basicDirectory, x, SearchOption.AllDirectories)).ToList() ?? new List<string>();
            var additionalGrains = Path.Combine(Directory.GetCurrentDirectory(), "grains");
            
            if (Directory.Exists(additionalGrains))
            {
                var additionalGrainsPaths =
                    Directory.GetFiles(additionalGrains, "*Grains.dll", SearchOption.AllDirectories);
                Console.WriteLine($"Additional grains: {string.Join(',', additionalGrainsPaths)}");
                asmPaths.AddRange(additionalGrainsPaths);
            }

            var asms = new List<Assembly>();
            if (asmPaths == null) 
                return asms.ToArray();
            
            foreach (var asmPath in asmPaths.Where(path => path.Contains("Grains") && !path.Contains("Interfaces")))
            {
                try
                {
                    var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(asmPath);
                    asms.Add(asm);
                    _logger.Debug($"Loaded assembly: {asmPath}!");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Can't load assembly: {asmPath}, {ex.Message}!");
                }
            }
            _logger.Info("Adding linked assmeblies for grains finished");

            return asms.ToArray();
        }

        private static IApplicationPartManagerWithAssemblies AddParts(IApplicationPartManager parts, SiloSettings siloSettings)
        {
            _logger.Info("Adding App Parts...");

            var results = parts
                .AddApplicationPart(typeof(DataChunk).Assembly)
                .AddApplicationPart(typeof(IX2IntegrationGrain).Assembly)
                .WithCodeGeneration();
            
            var linkedAsms = GetLinkedAssemblies(siloSettings);
            foreach (var asm in linkedAsms)
            {
                _logger.Info($"Loading app part \"{asm.GetName()}\"");
                try
                {
                    if (linkedAsms.Any(asm1 => asm1.GetName().Name == asm.GetName().Name && asm1.GetName().Version != asm.GetName().Version))
                    {
                        var existingAsms = linkedAsms.Where(asm1 => asm1.GetName().Name == asm.GetName().Name && asm1.GetName().Version != asm.GetName().Version);
                        var lastVersionAsm = existingAsms.OrderByDescending(asm1 => asm1.GetName().Version).FirstOrDefault();
                        if (lastVersionAsm != default)
                            results.AddApplicationPart(lastVersionAsm).WithCodeGeneration();
                    } else                    
                        results.AddApplicationPart(asm).WithCodeGeneration();

                } catch (Exception ex) {
                    _logger.Error($"Error loading app part \"{asm.GetName()}\": {ex.Message}");
                }
            }
            _logger.Info("App Parts loaded");

            return results;
        }
    }
}
