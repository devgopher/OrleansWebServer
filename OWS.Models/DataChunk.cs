using System;
using OWS.Models.Attributes;

namespace OWS.Models
{
    public class DataChunk
    {
        public DataChunk()
        {
            Id = Guid.NewGuid();
            SetDateTime(DateTime.UtcNow);
        }

        [Indexed]
        public Guid Id { get; set; }
        [Indexed]
        public long DateTimeTicks { get; set; }

        public static DateTime GetDateTime(long ticks) => new DateTime(ticks);
        public static DateTime GetDateTimeFromUnix(long unixMs) => DateTime.UnixEpoch.AddSeconds(unixMs);
        public DateTime GetDateTime() => GetDateTime(DateTimeTicks);

        public virtual void SetDateTime(DateTime dt) => DateTimeTicks = dt.Ticks;
    }
}
