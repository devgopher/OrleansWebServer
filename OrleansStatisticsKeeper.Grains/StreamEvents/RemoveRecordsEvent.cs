using System.Collections.Generic;

namespace OrleansStatisticsKeeper.Grains.StreamEvents
{
    public class RemoveRecordsEvent<T> : BasicEvent
    {
        public RemoveRecordsEvent() : base("REMOVERECORDS")
        {
        }

        public ICollection<T> Data { get; set; }
    }
}
