using Orleans;
using System.Threading.Tasks;

namespace OrleansWebServer.Backend.Grains.GrainsPool
{
    public interface IWebServerBackendGrainPool
    {
    }

    public interface IWebServerBackendGrainPool<TIn, TOut> : IWebServerBackendGrainPool
    {
        public Task<TOut> Execute(TIn request, GrainCancellationToken cancellationToken = default);
    }
}