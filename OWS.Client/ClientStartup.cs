using AsyncLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Networking.Shared;
using Orleans.Runtime;
using Orleans.Runtime.Messaging;
using OWS.Models.Settings;
using OWSUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OWS.Client.SiloDiscovery;

namespace OWS.Client
{
    public class ClientStartup
    {
        private readonly SiloSettings _siloSettings = new SiloSettings();
        private readonly OskSettings _oskSettings = new OskSettings();
        private static ClientStartup _instance = default;
        private static StatisticsClient _statisticsClient; 
        private IClusterClient _clusterClient;
        int _attempt = 0;

        public static ClientStartup Instance => _instance ??= new ClientStartup();

        private ClientStartup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection(nameof(SiloSettings)).Bind(_siloSettings);
            configuration.GetSection(nameof(OskSettings)).Bind(_oskSettings);

            var task = StartClientWithRetries(false);
            task.Wait();
            _statisticsClient = task.Result;
        }

        public StatisticsClient Client => _statisticsClient;

        public async Task<StatisticsClient> StartClientWithRetries(bool siloDiscovery)
        {
            _attempt = 0;
            _siloSettings.SiloAddresses ??= new List<string>();

            if (siloDiscovery)
            {
                var addresses = Discovery.ScanAddresses(_siloSettings.SiloPort);
                foreach (var address in addresses)
                    _siloSettings.SiloAddresses.Add(address);
            }
            else
                _siloSettings.SiloAddresses.Add(IpUtils.IpAddress().ToString());

            _clusterClient = new ClientBuilder()
                .UseStaticClustering(_siloSettings.SiloAddresses.Select(a => new IPEndPoint(IPAddress.Parse(a), _siloSettings.SiloPort)).ToArray())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _oskSettings.ClusterId;
                    options.ServiceId = _oskSettings.ServiceId;
                })
                .Configure<ConnectionOptions>(options =>
                {
                    options.OpenConnectionTimeout = TimeSpan.FromMinutes(1);
                    options.ProtocolVersion = NetworkProtocolVersion.Version2;
                    options.ConnectionRetryDelay = TimeSpan.FromMilliseconds(10);
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("OSKProvider")
                .Build();

            await _clusterClient.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");

            return new StatisticsClient(_clusterClient, new AsyncLogging.ConsoleLogger());
        }

        private async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException) && 
                exception.GetType() != typeof(SocketConnectionException) &&
                exception.GetType() != typeof(ConnectionFailedException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            ++_attempt;

            Console.WriteLine($"Cluster client attempt {_attempt} of {_siloSettings.InitializeAttemptsBeforeFailing} failed to connect to cluster. " +
                              $"Exception: {exception}");
            if (_attempt > _siloSettings.InitializeAttemptsBeforeFailing)
                return false;

            await Task.Delay(AttemptDelay());
            return true;
        }

        private TimeSpan AttemptDelay()
        {
            var x = 50 * _attempt;
            return TimeSpan.FromMilliseconds(50 * _attempt < 10000 ? x : 10000);
        }
    }
}
