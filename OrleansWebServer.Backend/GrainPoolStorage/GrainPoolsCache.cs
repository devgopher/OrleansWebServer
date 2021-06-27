using System;
using System.Collections.Generic;
using OrleansWebServer.Backend.Grains.GrainsPool;

namespace OrleansWebServer.Backend.GrainPoolStorage
{
    public class GrainPoolsCache
    {
        private readonly Dictionary<Type, IWebServerBackendGrainPool> _webExecutivePools =
            new Dictionary<Type, IWebServerBackendGrainPool>();

        private static GrainPoolsCache _instance = default;
        private static readonly object SyncObj = new object();

        private GrainPoolsCache()
        {
            Console.WriteLine($"{nameof(GrainPoolsCache)} constructor...");
        }

        public static GrainPoolsCache GetInstance()
        {
            if (_instance != default) 
                return _instance;
            lock (SyncObj)
            {
                _instance ??= new GrainPoolsCache();
            }

            return _instance;
        }

        public IWebServerBackendGrainPool this[Type type]
        {
            get => _webExecutivePools[type];
            set => _webExecutivePools[type] = value;
        }

        public bool ContainsType(Type type)
            => _webExecutivePools.ContainsKey(type);
    }
}