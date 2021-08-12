using Orleans;
using OWS.Grains.StreamEvents;
using System.Threading.Tasks;
using OWS.Grains.Models;
using OWS.Models;

namespace OWS.Grains.Interfaces
{
    public interface IManageStatisticsStreamGrain<T> : IGrainWithGuidKey
        where T : DataChunk
    {
        public Task Process(BasicEvent @event);

        public Task ProcessPutRecordsEvent(PutRecordsEvent<T> @event);
        public Task ProcessRemoveRecordsEvent(RemoveRecordsEvent<T> @event);
        public Task ProcessRemoveRecordsByConditionEvent(RemoveRecordsByConditionEvent<T> @event);
        public Task ProcessCleanRecordsEvent(CleanRecordsEvent<T> @event);
    }
}
