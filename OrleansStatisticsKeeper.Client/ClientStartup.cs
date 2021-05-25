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
using OrleansStatisticsKeeper.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Client
{
    public class ClientStartup
    {
        private readonly SiloSettings _siloSettings = new SiloSettings();
        private readonly OskSettings _oskSettings = new OskSettings();
        private int _attempt = 0;

        public ClientStartup()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection(nameof(SiloSettings)).Bind(_siloSettings);
            configuration.GetSection(nameof(OskSettings)).Bind(_oskSettings);
        }

        public StatisticsClient StartClientWithRetriesSync()
        {
            var statisticsClientTask = StartClientWithRetries();
            statisticsClientTask.Wait();
            return statisticsClientTask.Result;
        }

        public async Task<StatisticsClient> StartClientWithRetries()
        {
            _attempt = 0;
            _siloSettings.SiloAddresses ??= new List<string>();
            _siloSettings.SiloAddresses.Add(Utils.IpUtils.IpAddress().ToString());

            var innerClient = new ClientBuilder()
                .UseStaticClustering(_siloSettings.SiloAddresses.Select(a => new IPEndPoint(IPAddress.Parse(a), _siloSettings.SiloPort)).ToArray())
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = _oskSettings.ClusterId;
                    options.ServiceId = _oskSettings.ServiceId;
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("OSKProvider")
                .Build();

            await innerClient.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");

            return new StatisticsClient(innerClient, new NLogLogger(LogManager.GetCurrentClassLogger()));
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
