namespace OWS.Grains.StreamEvents
{
    public class CleanRecordsEvent<T> : BasicEvent
    {
        public CleanRecordsEvent() : base("CLEANRECORDS")
        {
        }
    }
}
