using System.Net;

namespace Runner.Interfaces
{
    public class Server
    {
        public string Name { get; set; }
        public IPAddress Ip { get; set; }
    }
}
