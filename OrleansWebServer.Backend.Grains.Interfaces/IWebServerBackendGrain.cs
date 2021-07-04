using System.Threading.Tasks;
using Orleans;

namespace OrleansWebServer.Backend.Grains.Interfaces
{
    public interface IWebServerBackendGrain : IGrainWithGuidKey
    {
        //public bool IsBusy { get; }
    }

    public interface IWebServerBackendGrain<IN, OUT> : IWebServerBackendGrain
    {
        //public bool IsBusy { get; }
        public Task<OUT> Execute(IN request, GrainCancellationToken cancellationToken);
    }
}
