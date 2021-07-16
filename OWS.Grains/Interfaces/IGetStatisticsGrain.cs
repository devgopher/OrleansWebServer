using System;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWS.Grains.Models;
using OWS.Models;

namespace OWS.Grains.Interfaces
{
    public interface IGetStatisticsGrain<T> : IGrainWithGuidKey
        where T : DataChunk
    {
        public Task<string> GetAllSerialized(GrainCancellationToken cancellationToken = null);
        public Task<T> GetFirst();
        public Task<T> GetLast();
        public Task<bool> Any();
        public Task<bool> Any(Func<bool,T> func);
    }
}
