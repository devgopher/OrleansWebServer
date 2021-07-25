using OWS.Backend.Grains.Models;

namespace OrleansWebServer.Grains.Models
{
    public class IntegralX2Request : OWSRequest
    {
        public double X0 { get; set; }
        public double X1 { get; set; }
        public double Accuracy { get; set; }
    }
}
