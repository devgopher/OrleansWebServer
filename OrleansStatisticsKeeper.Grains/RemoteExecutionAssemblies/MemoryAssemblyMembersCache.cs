using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils.Client;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public class MemoryAssemblyMembersCache : IAssemblyMembersCache
    {
        private readonly IAssemblyCache _assemblyCache;
        private readonly ConcurrentDictionary<string, Assembly> _innerCache
            = new ConcurrentDictionary<string, Assembly>();

        public MemoryAssemblyMembersCache(IAssemblyCache assemblyCache)
        {
            _assemblyCache = assemblyCache;
            AppDomain.CurrentDomain.AssemblyResolve += (object? sender, ResolveEventArgs args) =>
            {
                var task = assemblyCache.WaitFor(args.RequestingAssembly.FullName, 20000);
                task.Wait();

                var item = task.Result;
                if (item != null)
                {
                    //AppDomain.CurrentDomain.LO
                    var fullName = item.FullName;

                    var refs = item.GetReferencedAssemblies();
                    foreach (var @ref in refs)
                    {
                        if (AssemblyUtils.IsSystemAssembly(@ref) || AssemblyUtils.IsOskAssembly(@ref))
                            continue;
                        var asmDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                        var assembly = Assembly.LoadFile(Path.Combine(asmDirectory, $"{@ref.Name}.dll"));
                        //var types = assembly.GetTypes();
                        //foreach (var type in types)
                        //{
                        //    try
                        //    {
                        //        var tobj = Activator.CreateInstance(type);
                        //        if (tobj != default)
                        //            break;
                        //    }
                        //    catch (Exception)
                        //    {
                        //        // ignore
                        //    }
                        //}
                    }

                    return item;
                }

                return null;
            };
        }
        
        public static string GetFullKey(Type type) => $"{type.Assembly.GetName()}.{type.Name}";

        public void AddAssembly(Assembly assembly)
        {
            if (!_assemblyCache.Exists(assembly.FullName))
                _assemblyCache.Set(assembly);
            else
                _assemblyCache.Update(assembly);
            
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var fullKey = GetFullKey(type);
                _innerCache[fullKey] = assembly;
            }
        }

        public bool AssemblyExists(string assemblyFullName)
            => _assemblyCache.Exists(assemblyFullName);

        public Assembly GetAssembly(string assemblyFullName)
            => _assemblyCache.Get(assemblyFullName);

        public Assembly GetAssemblyForType(Type type)
            => _assemblyCache.Get(type.Assembly.FullName);

        public Assembly GetAssemblyForType(string type)
        {
            if (!_innerCache.Any(t => t.Key.Contains($".{type}")))
                return null;
            var kvp = _innerCache.FirstOrDefault(t => t.Key.Contains($".{type}"));
            return  kvp.Value;
        }
    }
}
