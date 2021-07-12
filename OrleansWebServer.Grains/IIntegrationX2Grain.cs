using OrleansWebServer.Backend.Grains.Interfaces;
using OrleansWebServer.Grains.Models;

namespace OrleansWebServer.Grains
{
    public interface IX2IntegrationGrain : IWebServerBackendGrain<IntegralX2Request, IntegralX2Response>
    {
    }
}