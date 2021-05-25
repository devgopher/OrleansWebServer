using Orleans;
using OrleansWebServer.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OrleansWebServer.Grains
{
    public class WeatherGrain : Grain, IWeatherGrain
    {
        public async Task<WeatherResponse> Execute(WeatherRequest request)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            return new WeatherResponse
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-20, 55),
                GrainId =  this.IdentityString
            };
        }
    }
}
