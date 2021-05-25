using System;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool.Exceptions
{
    public class GrainPoolException : Exception
    {
        public GrainPoolException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
