namespace OrleansStatisticsKeeper.Grains.StreamEvents
{
    public class CleanRecordsEvent<T> : BasicEvent
    {
        public CleanRecordsEvent() : base("CLEANRECORDS")
        {
        }
    }
}
