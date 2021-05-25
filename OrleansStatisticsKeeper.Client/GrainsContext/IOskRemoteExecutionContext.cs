using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Client.GrainsContext
{
    /// <summary>
    /// An interface for passing some keys|values for a grain (execution context).
    /// </summary>
    public interface IOskRemoteExecutionContext
    {
        public void SetValue(string name, object val);
        public object GetValue(string name);
        public T GetValue<T>(string name);
    }
}
