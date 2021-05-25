using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsPool<T> where T : class, IGrainWithGuidKey
    {
        protected readonly ConcurrentBag<T> _grains;
        private readonly StatisticsClient _client;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsPool(StatisticsClient client, int poolSize)
        {
            _grains = new ConcurrentBag<T>();
            _client = client;
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(_client.GetGrain<T>());
        }

        public virtual async Task Resize(int poolSize)
        {
            if (poolSize < 1)
                throw new GrainsPoolException($"poolSize should be >= 1!");
            
            if (_grains.Count >= poolSize)
                while (_grains.Count >= poolSize)
                    _grains.TryTake(out _);
            else
                while (_grains.Count < poolSize)
                    _grains.Add(_client.GetGrain<T>());
        }

        protected virtual async Task<int> GetGrainNumber() => rand.Next() % _grains.Count;
 
        protected virtual async Task<T> GetGrain() => _grains.ElementAt(await GetGrainNumber()) as T;
    }
}
