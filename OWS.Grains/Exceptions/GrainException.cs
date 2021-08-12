using System;
using System.Collections.Generic;
using System.Text;

namespace OWS.Grains.Exceptions
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
