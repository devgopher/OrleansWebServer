using System;
using Orleans;
using OrleansWebServer.Backend.Grains.Interfaces;
using System.Threading.Tasks;
using AsyncLogging;
using Microsoft.Extensions.Configuration;
using OrleansWebServer.Backend.Grains.Exceptions;

namespace OrleansWebServer.Backend.Grains
{
    public abstract class WebServerBackendGrain<TSettings, IN, OUT> : Grain, IWebServerBackendGrain<IN, OUT> 
        where TSettings : class, new()
    {
        private readonly IConfiguration _configuration;
        private readonly IAsyncLogger _logger;
        private TSettings _innerSettings;

        public WebServerBackendGrain(IConfiguration configuration, IAsyncLogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            BindConfiguration();
        }

        protected virtual void BindConfiguration()
        {
            try
            {
                _logger.Trace($"BindConfiguration(): starting...");
                _innerSettings = new TSettings();
                _configuration.GetSection(ConfigName).Bind(_innerSettings);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error binding a configuration for object of type {ConfigName}", ex);
                throw new WebServerBackendGrainException(ex.Message, ex);
            }

            _logger.Trace($"BindConfiguration(): successfull");
        }

        private string ConfigName => GetType().Name;

        public async Task<OUT> Execute(IN request)
        {
            _logger.Trace($"Execute() started");
            var result = await InnerExecute(request);
            _logger.Trace($"Execute() finished");
            return result;
        }

        public abstract Task<OUT> InnerExecute(IN request);
    }
}
