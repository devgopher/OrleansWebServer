using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utils.Client;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    [Serializable]
    public class GrainsExecutivePool : GrainsPool<IOskGrain>, IOskGrain
    {
        public GrainsExecutivePool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
        }

        public async Task<bool> GetIsLoaded(Type type) => _grains.All(g => g.GetIsLoaded(type).Result);

        public async Task<TOUT> Execute<TOUT>(Type type, string funcName, params object[] args)
            => await (await GetGrain()).Execute<TOUT>(type.Name, funcName, args);

        public async Task<TOUT> Execute<TOUT>(string className, string funcName, params object[] args) 
            => await (await GetGrain()).Execute<TOUT>(className, funcName, args);

        public async Task<TOUT> ExecuteWithContext<TOUT>(string className, string funcName, object context, params object[] args)
            => await(await GetGrain()).ExecuteWithContext<TOUT>(className, funcName, context, args);
        
        /// <summary>
        /// Loads a new assembly!
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public async Task LoadAssembly(Assembly assembly)
        {
            var asmBytes = AssemblyUtils.GetAssemblyBinary(assembly);
            var asmVersion = AssemblyUtils.GetAssemblyVersion(assembly);
            var asmFullname = assembly.FullName;

            await LoadAssembly(asmFullname, asmVersion, asmBytes);
        }
        
        /// <summary>
        /// Loads a new assembly!
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public async Task LoadAssembly(Type targetType, bool loadReferences = true)
        {
            var asmVersion = AssemblyUtils.GetAssemblyVersion(targetType);
            var asmFullname = AssemblyUtils.GetAssemblyName(targetType);
            var asmBytes = AssemblyUtils.GetAssemblyBinary(targetType);

            await LoadAssembly(asmFullname, asmVersion, asmBytes);

            if (loadReferences)
            {
                var references = AssemblyUtils.GetNonSystemReferencedAssemblies(asmFullname, 0, 3);
                foreach (var @ref in references)
                {
                    var asm = Assembly.Load(@ref);
                    await LoadAssembly(asm);
                }
            }

        }

        /// <summary>
        /// Loads a new assembly
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmPath"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, string asmPath)
        {
            var tasks = new List<Task>(_grains.Count);
            tasks.AddRange(_grains.Select(grain => grain.LoadAssembly(assemblyFullName, version, asmPath)));

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Loads a new assembly
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmBytes"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, byte[] asmBytes)
        {
            // TODO: change it onto direct loading into a cache!
            var tasks = new List<Task>(_grains.Count);
            tasks.AddRange(_grains.Select(grain => grain.LoadAssembly(assemblyFullName, version, asmBytes)));

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Resize is blocked temporary, because in this case we need to load all newly 
        /// added grains properly!
        /// </summary>
        /// <param name="poolSize"></param>
        /// <returns></returns>
        public override async Task Resize(int poolSize)
            => throw new NotSupportedException($"{nameof(Resize)} isn't supported for {nameof(GrainsExecutivePool)}!");
    }
}