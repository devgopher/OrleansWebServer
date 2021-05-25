using System;

namespace OrleansStatisticsKeeper.Grains.Exceptions
{
    public class GrainsPoolException : Exception
    {
        public GrainsPoolException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
