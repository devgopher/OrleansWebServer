using AsyncLogging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Hosting;
using OrleansWebServer.Grains;
using OWS.Models;
using OWS.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace OWS.SiloHost.Utils
{
    public static class AsmLoaderUtils
    {
        public static Assembly[] GetLinkedAssemblies(IAsyncLogger logger, SiloSettings siloSettings)
        {
            logger.Info("Adding linked assemblies for grains ...");
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            var basicDirectory = directoryInfo?.FullName;
            basicDirectory = directoryInfo != null ? directoryInfo.Parent?.Parent?.FullName : basicDirectory;

            var asmPaths = siloSettings.ModelsAssemblies?.SelectMany(x => Directory.GetFiles(basicDirectory, x, SearchOption.AllDirectories)).ToList() ?? new List<string>();
            var additionalGrains = Path.Combine(Directory.GetCurrentDirectory(), "grains");

            if (Directory.Exists(additionalGrains))
            {
                var additionalGrainsPaths =
                    Directory.GetFiles(additionalGrains, "*Grains.dll", SearchOption.AllDirectories);
                
                logger.Info($"Additional grains: {string.Join(',', additionalGrainsPaths)}");
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
                    logger.Debug($"Loaded assembly: {asmPath}!");
                }
                catch (Exception ex)
                {
                    logger.Error($"Can't load assembly: {asmPath}, {ex.Message}!");
                }
            }
            logger.Info("Adding linked assmeblies for grains finished");

            return asms.ToArray();
        }

        public static IApplicationPartManagerWithAssemblies AddParts(IAsyncLogger logger, IApplicationPartManager parts, SiloSettings siloSettings)
        {
            logger.Info("Adding App Parts...");

            var results = parts
                .AddApplicationPart(typeof(DataChunk).Assembly)
                .AddApplicationPart(typeof(IX2IntegrationGrain).Assembly)
                .WithCodeGeneration();

            var linkedAsms = GetLinkedAssemblies(logger, siloSettings);
            foreach (var asm in linkedAsms)
            {
                logger.Info($"Loading app part \"{asm.GetName()}\"");
                try
                {
                    if (linkedAsms.Any(asm1 => asm1.GetName().Name == asm.GetName().Name &&
                                               asm1.GetName().Version != asm.GetName().Version))
                    {
                        var existingAsms = linkedAsms.Where(asm1 => asm1.GetName().Name == asm.GetName().Name &&
                                                            asm1.GetName().Version != asm.GetName().Version);
                        var lastVersionAsm = existingAsms.OrderByDescending(asm1 => asm1.GetName().Version).FirstOrDefault();
                        if (lastVersionAsm != default)
                            results.AddApplicationPart(lastVersionAsm).WithCodeGeneration();
                    }
                    else
                        results.AddApplicationPart(asm).WithCodeGeneration();
                }
                catch (Exception ex)
                {
                    logger.Error($"Error loading app part \"{asm.GetName()}\": {ex.Message}");
                }
            }
            logger.Info("App Parts loaded");

            return results;
        }
    }
}
