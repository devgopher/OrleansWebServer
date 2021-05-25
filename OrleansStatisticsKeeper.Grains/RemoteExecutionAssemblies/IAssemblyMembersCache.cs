using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public interface IAssemblyMembersCache
    {
        public bool AssemblyExists(string assemblyFullName);
        public void AddAssembly(Assembly assembly);
        public Assembly GetAssembly(string assemblyFullName);
        public Assembly GetAssemblyForType(Type type);
        public Assembly GetAssemblyForType(string type);
    }
}
