using System;

namespace OWS.Grains.ClientGrainsPool.Exceptions
{
    public class GrainPoolException : Exception
    {
        public GrainPoolException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
