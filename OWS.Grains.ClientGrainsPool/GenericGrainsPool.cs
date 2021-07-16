using Orleans;
using OWS.Client;
using OWS.Grains.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace OWS.Grains.ClientGrainsPool
{
    public class GenericGrainsPool<T> where T : class, IGrainWithGuidKey
    {
        protected readonly ConcurrentBag<T> _grains;
        private readonly StatisticsClient _client;
        private readonly Random _rand = new Random(DateTime.Now.Millisecond);

        public GenericGrainsPool(StatisticsClient client, int poolSize)
        {
            _grains = new ConcurrentBag<T>();
            _client = client;
            for (var i = 0; i < poolSize; ++i)
                _grains.Add(_client.GetGrain<T>());
        }

        public virtual async Task Resize(int poolSize) =>
            await Task.Run(() =>
            {
                if (poolSize < 1)
                    throw new GrainsPoolException($"poolSize should be >= 1!");

                if (_grains.Count >= poolSize)
                    while (_grains.Count >= poolSize)
                        _grains.TryTake(out _);
                else
                    while (_grains.Count < poolSize)
                        _grains.Add(_client.GetGrain<T>());
            });

        public int Count => _grains?.Count ?? 0;

        protected virtual async Task<int> GetGrainNumber() => _rand.Next() % _grains.Count;
 
        protected virtual async Task<T> GetGrain() => _grains.ElementAt(await GetGrainNumber()) as T;
    }
}
