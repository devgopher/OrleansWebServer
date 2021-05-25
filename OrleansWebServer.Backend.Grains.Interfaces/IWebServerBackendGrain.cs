using System.Threading.Tasks;
using Orleans;

namespace OrleansWebServer.Backend.Grains.Interfaces
{
    public interface IWebServerBackendGrain : IGrainWithGuidKey
    {
    }
    public interface IWebServerBackendGrain<IN, OUT> : IWebServerBackendGrain
    {
        public Task<OUT> Execute(IN request);
    }
}
