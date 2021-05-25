using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using AsyncLogging;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Client
{
    public class StatisticsClient : IDisposable
    {
        private readonly IClusterClient _client;
        private readonly IAsyncLogger _logger;

        public StatisticsClient(IClusterClient client, IAsyncLogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public IManageStatisticsGrain<T> AddStatisticsGrain<T>() where T : DataChunk 
            => _client.GetGrain<IManageStatisticsGrain<T>>(Guid.NewGuid());

        public IGetStatisticsGrain<T> GetStatisticsGrain<T>() where T : DataChunk
            => _client.GetGrain<IGetStatisticsGrain<T>>(Guid.NewGuid());

        public IOskGrain GetExecutiveGrain()
            => _client.GetGrain<IOskGrain>(Guid.NewGuid());

        public T GetGrain<T>() where T : IGrainWithGuidKey
            => _client.GetGrain<T>(Guid.NewGuid());

        public void Dispose() => _client?.Dispose();
    }
}
