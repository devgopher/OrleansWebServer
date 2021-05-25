using Orleans;
using OrleansStatisticsKeeper.Grains.StreamEvents;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.Interfaces
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
