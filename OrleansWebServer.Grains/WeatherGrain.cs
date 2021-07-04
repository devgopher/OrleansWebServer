using Orleans;
using OrleansWebServer.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OrleansWebServer.Grains
{
    public class WeatherGrain : Grain, IWeatherGrain
    {
        public bool IsBusy => throw new NotImplementedException();

        public async Task<WeatherResponse> Execute(WeatherRequest request, GrainCancellationToken cancellationToken)
        {
            var rng = new Random(DateTime.Now.Millisecond);
            return new WeatherResponse
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-50, 55),
                GrainId = this.IdentityString
            };
        }
    }
}
