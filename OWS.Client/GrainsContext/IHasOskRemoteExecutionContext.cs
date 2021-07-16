using System;
using System.Collections.Generic;
using System.Text;

namespace OWS.Client.GrainsContext
{
    public interface IHasOskRemoteExecutionContext
    {
        public IOskRemoteExecutionContext Context { get; }
    }
}
