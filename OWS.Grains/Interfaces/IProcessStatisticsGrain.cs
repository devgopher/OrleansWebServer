using System;
using System.Threading.Tasks;
using OWS.Models;

namespace OWS.Grains.Interfaces
{
    public interface IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public Task Process(Func<T, bool> func);
    }
}
