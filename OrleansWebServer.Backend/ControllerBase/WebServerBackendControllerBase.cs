﻿using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansWebServer.Backend.Settings;
using System.Threading.Tasks;
using OrleansWebServer.Backend.Grains.GrainsPool;
using OrleansWebServer.Backend.Grains.Interfaces;

namespace OrleansWebServer.Backend.ControllerBase
{
    public class WebServerBackendControllerBase<TGrain, IN, OUT>
        where  TGrain : class, IGrainWithGuidKey, IWebServerBackendGrain<IN,OUT>
    {
        private readonly WebServerBackendGrainPool<TGrain, IN , OUT> _grainPool;

        public WebServerBackendControllerBase(WebServerBackendSettings settings, StatisticsClient client)
        {
        }

        public virtual async Task<V> Execute<T, V>(T request) => await _grainPool?.Execute<T, V>(request);
    }
}