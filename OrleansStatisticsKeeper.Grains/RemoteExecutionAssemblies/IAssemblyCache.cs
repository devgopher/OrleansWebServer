using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public interface IAssemblyCache
    {
        public bool Exists(string fullName);
        public Assembly Get(string fullName);
        public Task<Assembly> WaitFor(string fullName, int timeoutInMs);
        public void Set(Assembly assembly);
        public void Update(Assembly assembly);
    }
}
