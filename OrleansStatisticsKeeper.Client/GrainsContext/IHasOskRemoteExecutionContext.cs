using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Client.GrainsContext
{
    public interface IHasOskRemoteExecutionContext
    {
        public IOskRemoteExecutionContext Context { get; }
    }
}
