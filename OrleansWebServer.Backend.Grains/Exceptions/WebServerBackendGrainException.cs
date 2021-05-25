using System;

namespace OrleansWebServer.Backend.Grains.Exceptions
{
    public class WebServerBackendGrainException : Exception
    {
        public WebServerBackendGrainException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}
