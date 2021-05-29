using System.Threading.Tasks;
using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains;
using OrleansWebServer.Backend.Grains.Interfaces;

namespace OrleansWebServer.Backend.Grains.GrainsPool
{
    public class WebServerBackendGrainPool<TGrain, TIn, TOut> : ClientGrainsPool<TGrain>, IWebServerBackendGrain<TIn, TOut>, IWebServerBackendGrainPool<TIn, TOut>
        where TGrain : class, IWebServerBackendGrain<TIn, TOut>, IGrainWithGuidKey
    {
        public WebServerBackendGrainPool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
        }

        public async Task<TOut> Execute(TIn request)
            => await (await GetGrain()).Execute(request);

        public Task<V> Execute<T, V>(T request)
        {
            throw new System.NotImplementedException();
        }
    }
}
