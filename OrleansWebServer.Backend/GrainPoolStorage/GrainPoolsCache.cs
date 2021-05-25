using System;
using System.Collections.Generic;
using OrleansWebServer.Backend.Grains.GrainsPool;

namespace OrleansWebServer.Backend.GrainPoolStorage
{
    public class GrainPoolsCache
    {
        private readonly Dictionary<Type, IWebServerBackendGrainPool> _webExecutivePools =
            new Dictionary<Type, IWebServerBackendGrainPool>();

        private static GrainPoolsCache instance = default;
        private static object syncObj = new object();

        private GrainPoolsCache()
        {

        }

        public static GrainPoolsCache GetInstance()
        {
            if (instance == default)
            {
                lock (syncObj)
                {
                    instance ??= new GrainPoolsCache();
                }
            }

            return instance;
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