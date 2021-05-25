using Microsoft.AspNetCore.Mvc;
using OrleansWebServer.Backend;
using OrleansWebServer.Grains.Models;
using System.Threading.Tasks;

namespace OrleansWebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WebServerBackend _wg;

        public WeatherForecastController(WebServerBackend wg) => _wg = wg;

        [HttpPost]
        public async Task<WeatherResponse> Get(WeatherRequest wr)
            => await _wg.SendRequest<WeatherRequest, WeatherResponse>(wr);
    }
}
