using System;

namespace OWS.Grains.StreamEvents
{
    public class RemoveRecordsByConditionEvent<T> : BasicEvent
    {
        public RemoveRecordsByConditionEvent() : base("REMOVECONDRECORDS")
        {
        }

        public Func<T, bool> ConditionFunc { get; set; }
    }
}
