﻿using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansWebServer.Backend.Grains.Interfaces;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;

namespace OrleansWebServer.Backend.Grains.GrainsPool
{
    public class WebServerBackendGrainPool<TGrain, TIn, TOut> : GenericGrainsPool<TGrain>, IWebServerBackendGrain<TIn, TOut>, IWebServerBackendGrainPool<TIn, TOut>
        where TGrain : class, IWebServerBackendGrain<TIn, TOut>, IGrainWithGuidKey
    {
        public WebServerBackendGrainPool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
        }

        public bool IsBusy => throw new System.NotImplementedException();

        public async Task<TOut> Execute(TIn request, GrainCancellationToken cancellationToken = default)
        {
            var grain = await GetGrain();
            //while (grain.IsBusy)
            //    grain = await GetGrain();

            return await grain.Execute(request, cancellationToken);
        }

        public Task<V> Execute<T, V>(T request)
        {
            throw new System.NotImplementedException();
        }
    }
}
