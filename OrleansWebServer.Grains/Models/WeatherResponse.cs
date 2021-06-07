using System;

namespace OrleansWebServer.Grains.Models
{
    public class WeatherResponse
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }


        public string GrainId { get; set; }
}
}
