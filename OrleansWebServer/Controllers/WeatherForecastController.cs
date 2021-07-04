using Microsoft.AspNetCore.Mvc;
using Orleans;
using OrleansWebServer.Backend;
using OrleansWebServer.Grains.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OrleansWebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly WebServerBackend _wg;
        private readonly GrainCancellationTokenSource source = new GrainCancellationTokenSource();

        public TestController(WebServerBackend wg) => _wg = wg;

        [HttpPost("[action]")]
        public async Task<WeatherResponse> GetWeather(WeatherRequest wr, CancellationToken cancellationToken = default)
        {
            return await _wg.SendRequest<WeatherRequest, WeatherResponse>(wr, cancellationToken);
        }

        [HttpPost("[action]")]
        public async Task<IntegralX2Response> GetIntegral(IntegralX2Request wr, CancellationToken cancellationToken)
            => await _wg.SendRequest<IntegralX2Request, IntegralX2Response>(wr, cancellationToken);
    }
}
