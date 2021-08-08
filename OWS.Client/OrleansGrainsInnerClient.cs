using AsyncLogging;
using Orleans;
using OWS.Grains.Interfaces;
using OWS.Models;
using System;

namespace OWS.Client
{
    /// <summary>
    /// This client connects a Web Server backend to Grains 
    /// </summary>
    public class OrleansGrainsInnerClient : IDisposable
    {
        private readonly IClusterClient _client;
        protected readonly IAsyncLogger _logger;

        public OrleansGrainsInnerClient(IClusterClient client, IAsyncLogger logger)
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
