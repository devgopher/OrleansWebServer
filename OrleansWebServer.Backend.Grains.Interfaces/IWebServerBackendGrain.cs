using Orleans;
using OWS.Backend.Grains.Models;
using System.Threading.Tasks;

namespace OrleansWebServer.Backend.Grains.Interfaces
{
    public interface IWebServerBackendGrain : IGrainWithGuidKey
    {
        //public bool IsBusy { get; }
    }

    public interface IWebServerBackendGrain<IN, OUT> : IWebServerBackendGrain
        where IN : OWSRequest
        where OUT : OWSResponse
    {
        //public bool IsBusy { get; }
        public Task<OUT> Execute(IN request, GrainCancellationToken cancellationToken);
    }
}
