using McGuireV10.OrleansDistributedCache;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OWS.Backend.Grains.Models;
using OWS.Services.Caching.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace OWS.Services.Caching
{
    /// <summary>
    /// Response caching service
    /// </summary>
    public class OWSCachingService
    {
        private readonly OrleansDistributedCacheService _innerService;
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        public OWSCachingService(OrleansDistributedCacheService innerService)
            => _innerService = innerService;

        private string ProduceKey(OWSRequest request)
        {
            var ret = string.Empty;
            var attrs = request.GetType().GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                var attrValue = attr as CacheableAttribute;
                if (attrValue == default)
                    continue;

                if (attrValue.Cache)
                {
                    var props = request.GetType().GetProperties();
                    var propNames = new List<string>(5);

                    foreach (var prop in props)
                    {
                        var propAttrs = prop.PropertyType.GetCustomAttributes(false);
                        if (propAttrs.Any(pa => pa is CacheElementAttribute))
                            propNames.Add(prop.Name);
                    }

                    ret = JsonConvert.SerializeObject(propNames);
                    break;
                }
            }

            return ret;
        }

        private byte[] SerializeResponse(OWSResponse response)
        {
            using MemoryStream ms = new MemoryStream();
            _formatter.Serialize(ms, response);

            return ms.ToArray();
        }

        private T DeserializeResponse<T>(byte[] input)
            where T : class
        {
            using MemoryStream ms = new MemoryStream(input);
            var srcObject = _formatter.Deserialize(ms);
            var ret = srcObject as T;

            return ret;
        }

        /// <summary>
        /// Sets a cache object
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task Set(OWSRequest request, OWSResponse response)
        {
            await ProcessRequest<OWSResponse>(request, ref response, async (request, response) =>
            {
                var key = ProduceKey(request);
                var val = SerializeResponse(response);
                await _innerService.SetAsync(key, val, new DistributedCacheEntryOptions());

                return response;
            });
        }

        /// <summary>
        /// Gets a cache object
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<TResponse> Get<TResponse>(OWSRequest request)
            where TResponse : OWSResponse, new()
        {
            var response = new TResponse();

            return await ProcessRequest(request, ref response, async (request, response) =>
            {
                var key = ProduceKey(request);
                var serializedResponse = await  _innerService.GetAsync(key);
                if (serializedResponse == default)
                    return default;

                var val = DeserializeResponse<TResponse>(serializedResponse);
                return val;
            });
        }

        private Task<TOut> ProcessRequest<TOut>(OWSRequest request, ref TOut response, Func<OWSRequest, TOut, Task<TOut>> action)
            where TOut : OWSResponse
        {
            var attrs = request.GetType().GetCustomAttributes(false);
            foreach (var attr in attrs)
            {
                var attrValue = attr as CacheableAttribute;
                if (attrValue == default)
                    continue;

                if (attrValue.Cache)
                    return action(request, response);
            }

            throw new Exception($"Can't process a request: {request.RequestId}");
        }
    }
}
