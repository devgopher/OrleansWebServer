using System;

namespace OWS.Grains.Exceptions
{
    public class GrainsPoolException : Exception
    {
        public GrainsPoolException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
