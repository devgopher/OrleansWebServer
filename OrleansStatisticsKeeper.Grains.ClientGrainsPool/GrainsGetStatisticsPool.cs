using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsGetStatisticsPool<T> : GrainsPool<IGetStatisticsGrain<T>>, IGetStatisticsGrain<T>
        where T : DataChunk
    {
        public GrainsGetStatisticsPool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
        }

        public async Task<string> GetAllSerialized(GrainCancellationToken cancellationToken = null)
            => await (await GetGrain()).GetAllSerialized();

        public async Task<ICollection<T>> GetAllCollection(GrainCancellationToken cancellationToken = null)
            => JsonConvert.DeserializeObject<ICollection<T>>(await GetAllSerialized(cancellationToken));

        public async Task<T> GetFirst() => await (await GetGrain()).GetFirst();

        public async Task<T> GetLast() => await (await GetGrain()).GetLast();

        public async Task<bool> Any() => await (await GetGrain()).Any();
        public Task<bool> Any(Func<bool, T> func)
        {
            throw new NotImplementedException();
        }
    }
}