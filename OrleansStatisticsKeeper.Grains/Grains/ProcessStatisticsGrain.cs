using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class ProcessStatisticsGrain<T> : Grain, IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public async Task Process(Func<T, bool> func)
        {

        }
    }
}
