using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.Exceptions
{
    [Serializable]
    public class GrainException : Exception
    {
        public GrainException() : base()
        {

        }

        public GrainException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }
}
