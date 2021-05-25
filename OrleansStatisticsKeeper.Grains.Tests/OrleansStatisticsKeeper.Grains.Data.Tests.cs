using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OrleansStatisticsKeeper.Grains.Grains;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Tests.Models;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Globalization;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Models.Settings;
using AsyncLogging;
using OrleansStatisticsKeeper.Grains.Tests.TestClasses;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using System.IO;
using OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies;
using Utils.Client;
using OrleansStatisticsKeeper.Client.GrainsContext;

namespace OrleansStatisticsKeeper.Grains.Tests
{
    public class Tests
    {
        private IManageStatisticsGrain<TestModel> _addStatisticsGrain;
        private IGetStatisticsGrain<TestModel> _getStatisticsGrain;
        private GrainsExecutivePool _grainsExecutivePool;
        private IOskGrain _executiveGrain;
        private MongoUtils _mongoUtils;
        private OskSettings _oskSettings = new OskSettings();
        private IAsyncLogger _logger;
        private IOskRemoteExecutionContext _grainsContext;

        [SetUp]
        public async Task Setup()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection("OskSettings").Bind(_oskSettings);

            _mongoUtils = new MongoUtils(_oskSettings);
            _addStatisticsGrain = new MongoManageStatisticsGrain<TestModel>(_mongoUtils);
            _getStatisticsGrain = new MongoGetStatisticsGrain<TestModel>(_mongoUtils, new NLogLogger());
            _grainsContext = new GenericGrainsContext();
            _logger = new NLogLogger();
            _executiveGrain = new GenericExecutiveGrain(new MemoryAssemblyMembersCache(new MemoryAssemblyCache()), _logger);
            //var clt = new ClientStartup();
            //var client = await clt.StartClientWithRetries();
            //_grainsExecutivePool = new GrainsExecutivePool(client, 10);
            await FillData();
        }

        public async Task FillData()
        {
            if (!await _getStatisticsGrain.Any())
            {
                for (var i = 0; i < 1000000; ++i)
                    await _addStatisticsGrain.Put(new TestModel()
                    {
                        Text = $"TTTTT_{i}"
                    });
            }
        }

        [Test]
        public async Task GetStatisticsNotEmpty()
        {
           Assert.IsTrue(await _getStatisticsGrain.Any());
        }

        [Test]
        [TestCase("TEXT1")]
        public async Task PutStatistics(string text)
        {
            var insertText = text + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            await _addStatisticsGrain.Put(new TestModel() {Text = insertText});
            Assert.IsTrue(await _getStatisticsGrain.AnyAsync(x => x.Text == insertText));
        }

        [Test]
        [TestCase(102)]
        public async Task ExecuteMethodFromAssemblyTest(int a)
        {
            var asmPath = Path.Combine(Directory.GetCurrentDirectory(), "TestAssembly.dll");
            await _executiveGrain.LoadAssembly(asmPath, AssemblyUtils.GetAssemblyVersion(asmPath), asmPath);
            var ret = await _executiveGrain.Execute<double>(nameof(TestAssembly.TestClass), nameof(TestAssembly.TestClass.Pow2), a);
            Assert.AreEqual(ret, TestAssembly.TestStaticClass.Pow2(a));
        }

        [Test]
        [TestCase(5)]
        public async Task ExecuteStaticMethodFromAssemblyTest(int a)
        {
            var asmPath = Path.Combine(Directory.GetCurrentDirectory(), "TestAssembly.dll");
            await _executiveGrain.LoadAssembly(AssemblyUtils.GetAssemblyName(asmPath),
                AssemblyUtils.GetAssemblyVersion(asmPath), asmPath);
            var ret = await _executiveGrain.Execute<double>(nameof(TestAssembly.TestStaticClass), 
                nameof(TestAssembly.TestStaticClass.Pow2), a);
            Assert.AreEqual(ret, TestAssembly.TestStaticClass.Pow2(a));
        }

        //[Test]
        //[TestCase("TEXT1")]
        //public async Task ExecutiveGrainsTest(string text)
        //{
        //    var insertText = text + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        //    var ret = await _executiveGrain.Execute<string>((t) => t, text);
        //    Assert.AreEqual(ret, text);
        //}

        //[Test]
        //[TestCase(5, 3)]
        //public async Task ExecutiveGrainsWithClassTest(int a, int b)
        //{
        //    var tc = new TestExecutionContext();

        //    var ret = await _executiveGrain.Execute<int, int, double>((t1, t2) => tc.Test(t1, t2), a, b);
        //    Assert.AreEqual(ret, tc.Test(a,b));
        //}

        //[Test]
        //[TestCase(5, 3)]
        //public async Task ExecutiveGrainsWithClassPoolTest(int a, int b)
        //{
        //    var tc = new TestExecutionContext();

        //    var ret = await _grainsExecutivePool.Execute<int, int, double>((t1, t2) => tc.Test(t1, t2), a, b);
        //    Assert.AreEqual(ret, tc.Test(a, b));
        //}
    }
}