using System;

namespace OrleansWebServer.Backend.Exceptions
{
    public class WebServerBackendException : Exception
    {
        public WebServerBackendException(string message, Exception inner = default) : base(message, inner)
        {
        }
    }
}
