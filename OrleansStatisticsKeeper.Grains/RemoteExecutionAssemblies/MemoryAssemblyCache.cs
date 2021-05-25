using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Client;
using Utils.Crypto;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public class MemoryAssemblyCache : IAssemblyCache
    {
        private readonly ConcurrentDictionary<string, Assembly> _assemblies
            = new ConcurrentDictionary<string, Assembly>();

        public bool Exists(string fullName)
            => _assemblies.Any(a => (a.Key == fullName));

        public Assembly Get(string fullName)
        {
            if (!Exists(fullName))
                return default;
            var kvp = _assemblies.FirstOrDefault(a => (a.Key == fullName));
            return kvp.Value;
        }

        public void Set(Assembly assembly)
        {
            if (!_assemblies.ContainsKey(assembly.FullName))
                _assemblies[assembly.FullName] = assembly;
        }

        public async Task<Assembly> WaitFor(string fullName, int timeoutInMs = 20000)
        {
            DateTime current = DateTime.Now;
            while (!Exists(fullName) && (DateTime.Now - current).TotalMilliseconds > timeoutInMs)
                Thread.Sleep(10);

            if (!Exists(fullName))
                return null;
            
            var kvp = _assemblies.FirstOrDefault(a => (a.Key == fullName));
            return kvp.Value;
        }

        public void Update(Assembly assembly)
        {
            if (_assemblies.ContainsKey(assembly.FullName))
            {
                var existingAsm = Get(assembly.FullName);
                if (!HashUtils.CompareHashes(HashUtils.HashForAssembly(assembly), HashUtils.HashForAssembly(existingAsm))) 
                {
                    // yeah, I think we need a lock, 'cause of we need to use the same copy of an assembly for all grains simultaneouly!
                    lock (_assemblies) 
                        _assemblies[assembly.FullName] = assembly;
                }
            }
        }
    }
}
