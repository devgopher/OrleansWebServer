namespace OrleansStatisticsKeeper.Grains.StreamEvents
{
    public class BasicEvent
    {
        public virtual string Type { get; private set; }

        public BasicEvent(string type)
        {
            Type = type;
        }
    }
}
