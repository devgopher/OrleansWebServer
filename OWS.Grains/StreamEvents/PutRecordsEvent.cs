using System.Collections.Generic;

namespace OWS.Grains.StreamEvents
{
    public class PutRecordsEvent<T> : BasicEvent
    {
        public PutRecordsEvent() : base("PUTRECORDS")
        {
        }

        public ICollection<T> Data { get; set; }
    }
}
