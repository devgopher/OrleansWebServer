using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Utils.Process;

namespace OrleansStatisticsKeeper.SiloHost.Utils
{
    class CpuOptimizer
    {
        private const int MaxThreadCount = 1000;
        private const int MinThreadCount = 10;
        private static int _currentThreadCount = 50;

        public static async Task Start(int percent, CancellationToken cancellationToken)
        {
            ThreadPool.SetMaxThreads(MaxThreadCount, 2 * MaxThreadCount);
            ThreadPool.SetMinThreads(MinThreadCount, 2 * MinThreadCount);

            if (percent < 0 || percent > 100)
                throw new InvalidOperationException("Percent must be in a diapazone [0..100]!");

            using (var cpuCounter = ProcessCpuCounter.GetPerfCounterForProcessId(Process.GetCurrentProcess().Id))
            {
                while (/*!cancellationToken.IsCancellationRequested*/true)
                {
                    var cpuValue = cpuCounter.NextValue();
                    Console.WriteLine($"CPU USAGE: {cpuValue}: {percent}!");
                    if (cpuValue > percent)
                    {
                        Console.WriteLine($"CPU BOUNDS EXCEEDED: {cpuValue}: {percent}!");
                        while (_currentThreadCount > MinThreadCount && cpuValue > percent)
                        {
                            var delta = (int)(0.1 * _currentThreadCount);
                            delta = delta > 1 ? delta : 1;
                            _currentThreadCount -= delta;
                            _currentThreadCount = _currentThreadCount > MinThreadCount ? _currentThreadCount : MinThreadCount;
                            ThreadPool.SetMaxThreads(_currentThreadCount, 2 * _currentThreadCount);
                            Thread.Sleep(500);
                            cpuValue = cpuCounter.NextValue();
                        }
                    }
                    else if (cpuCounter.NextValue() < 0.5 * percent)
                    {
                        Console.WriteLine($"CPU IS UNDERUSED: {cpuValue}: {percent}!");
                        while (_currentThreadCount > MinThreadCount && cpuCounter.NextValue() < percent)
                        {
                            var delta = (int)(0.1 * _currentThreadCount);
                            delta = delta > 1 ? delta : 1;
                            _currentThreadCount += delta;
                            _currentThreadCount = _currentThreadCount < MaxThreadCount ? _currentThreadCount : MaxThreadCount;
                            ThreadPool.SetMaxThreads(_currentThreadCount, 2 * _currentThreadCount);
                            Thread.Sleep(500);
                            cpuValue = cpuCounter.NextValue();
                        }
                    }

                    Console.WriteLine($"CURRENT THREAD COUNT: {_currentThreadCount}");
                    Thread.Sleep(15000);
                }
            }
        }
    }
}
