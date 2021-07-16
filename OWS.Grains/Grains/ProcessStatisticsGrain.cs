using Orleans;
using OWS.Grains.Interfaces;
using OWS.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OWS.Grains.Grains
{
    public class ProcessStatisticsGrain<T> : Grain, IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public async Task Process(Func<T, bool> func)
        {

        }
    }
}
