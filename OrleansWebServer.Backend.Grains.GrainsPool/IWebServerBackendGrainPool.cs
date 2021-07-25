using Orleans;
using OWS.Backend.Grains.Models;
using System.Threading.Tasks;

namespace OrleansWebServer.Backend.Grains.GrainsPool
{
    public interface IWebServerBackendGrainPool
    {
    }

    public interface IWebServerBackendGrainPool<TIn, TOut> : IWebServerBackendGrainPool
        where TIn: OWSRequest
        where TOut: OWSResponse
    {
        public Task<TOut> Execute(TIn request, GrainCancellationToken cancellationToken = default);
    }
}