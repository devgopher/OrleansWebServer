using OWS.Backend.Grains.Models;
using OWS.Services.Caching.Attributes;
using System.Collections.Generic;

namespace OrleansWebServer.Grains.Models
{
    [Cacheable(Cache = true)]
    public class IntegralX2Request : OWSRequest
    {
        [CacheElement]
        public double X0 { get; set; }
        [CacheElement]
        public double X1 { get; set; }
        [CacheElement]
        public double Accuracy { get; set; }
    }
}
