using Orleans;
using OrleansStatisticsKeeper.Client;
using OrleansWebServer.Backend.Exceptions;
using OrleansWebServer.Backend.GrainPoolStorage;
using OrleansWebServer.Backend.Grains.GrainsPool;
using OrleansWebServer.Backend.Grains.Interfaces;
using OrleansWebServer.Backend.Settings;
using System;
using System.Threading.Tasks;

namespace OrleansWebServer.Backend
{
    public class WebServerBackend : IDisposable
    {
        private readonly GrainPoolsCache _webExecutivePools;
        private readonly StatisticsClient _client;
        private readonly WebServerBackendSettings _settings;
        private readonly AsyncLogging.AsyncLogging _logger;

        public WebServerBackend(WebServerBackendSettings settings,
            AsyncLogging.AsyncLogging logger ) //: base(settings.SchedulerSettings)
        {
            var clt = new ClientStartup();
            _client = clt.StartClientWithRetriesSync();
            _webExecutivePools = GrainPoolsCache.GetInstance();
            _settings = settings;
            _logger = logger;
        }

        /// <summary>
        /// Registers a request and grain type for it's processing
        /// </summary>
        /// <typeparam name="TGrain">A type of grain</typeparam>
        /// <typeparam name="TIn">Input data type</typeparam>
        /// <typeparam name="TOut">Output data type</typeparam>
        /// <returns></returns>
        public void RegisterRequest<TGrain, TIn, TOut>()
            where TGrain : class, IWebServerBackendGrain<TIn, TOut>, IGrainWithGuidKey
        {
            _logger.Info($"{nameof(RegisterRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name}: started...");
            if (_webExecutivePools.ContainsType(typeof(TIn)))
                throw new WebServerBackendException($"{nameof(RegisterRequest)} for {typeof(TIn).Name} with output" +
                                                    $" of type: {typeof(TOut).Name} : input type is ALREADY registered!");
            _webExecutivePools[typeof(TIn)] = new WebServerBackendGrainPool<TGrain, TIn, TOut>(_client, _settings.Nodes);

            _logger.Info($"{nameof(RegisterRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name}: completed");
        }

        /// <summary>
        /// Sends request using a suitable Grain 
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TOut> SendRequest<TIn, TOut>(TIn request)
        {
            _logger.Info($"{nameof(SendRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name}: started...");

            if (!_webExecutivePools.ContainsType(typeof(TIn)))
                throw new WebServerBackendException(
                    $"{nameof(SendRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name} : input type is NOT registered!");

            if (!(_webExecutivePools[typeof(TIn)] is IWebServerBackendGrainPool<TIn, TOut> pool))
                throw new WebServerBackendException($"{nameof(SendRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name} : " +
                                                    $"grain doesn't accords to {typeof(IWebServerBackendGrainPool<TIn, TOut>)}!");

            var result = await pool.Execute(request);

            _logger.Info($"{nameof(SendRequest)} for {typeof(TIn).Name} with output of type: {typeof(TOut).Name} got result");
            return result;
        }

        public void Dispose() => _client?.Dispose();
    }
}
