using System;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public Task Process(Func<T, bool> func);
    }
}
