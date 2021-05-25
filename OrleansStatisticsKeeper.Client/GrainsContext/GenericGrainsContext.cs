using OrleansStatisticsKeeper.Grains.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Client.GrainsContext
{
    public class GenericGrainsContext : IOskRemoteExecutionContext
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(20);
        public object GetValue(string name)
            => _values[name];

        public T GetValue<T>(string name)
        {
            if (_values.ContainsKey(name))
                return default;
                //throw new GrainException($"Can't find a context value for key {name}!");

            if (!typeof(T).IsValueType)
                if (!(_values[name] is T))
                    return default;

            return (T)_values[name];
        }

        public void SetValue(string name, object val)
            => _values[name] = val;
    }
}
