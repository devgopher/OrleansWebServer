using OrleansWebServer.Backend.Grains.Interfaces;
using OrleansWebServer.Grains.Models;

namespace OrleansWebServer.Grains
{
    public interface IWeatherGrain : IWebServerBackendGrain<WeatherRequest, WeatherResponse>
    {
    }
}