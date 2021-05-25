using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils.Process
{
    public static class ProcessCpuCounter
    {
        private static Dictionary<int, PerformanceCounter> performanceCounters = new Dictionary<int, PerformanceCounter>();

        public static PerformanceCounter GetPerfCounterForProcessId(int processId, string processCounterName = "% Processor Time")
        {
            string instance = GetInstanceNameForProcessId(processId);
            if (string.IsNullOrEmpty(instance))
                return null;

            if (!performanceCounters.ContainsKey(processId))
                performanceCounters[processId] = new PerformanceCounter("Process", processCounterName, instance);

            return performanceCounters[processId];
        }

        public static string GetInstanceNameForProcessId(int processId)
        {
            var process = System.Diagnostics.Process.GetProcessById(processId);
            string processName = Path.GetFileNameWithoutExtension(process.ProcessName);

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Process");
            string[] instances = cat.GetInstanceNames()
                .Where(inst => inst.StartsWith(processName))
                .ToArray();

            foreach (string instance in instances)
            {
                using (PerformanceCounter cnt = new PerformanceCounter("Process",
                    "ID Process", instance, true))
                {
                    int val = (int)cnt.RawValue;
                    if (val == processId)
                    {
                        return instance;
                    }
                }
            }
            return null;
        }
    }
}
