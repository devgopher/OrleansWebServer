using Orleans;
using Orleans.Streams;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.StreamEvents;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public abstract class GenericManageStatisticsStreamGrain<T> : Grain, IManageStatisticsStreamGrain<T>
        where T : DataChunk
    {
        public async Task Process(BasicEvent @event)
        {
            switch (@event)
            {
                case PutRecordsEvent<T> putRecordsEvent:
                    await ProcessPutRecordsEvent(putRecordsEvent);
                    break;
                case RemoveRecordsEvent<T> removeRecordsEvent:
                    await ProcessRemoveRecordsEvent(removeRecordsEvent);
                    break;
                case RemoveRecordsByConditionEvent<T> removeRecordsByConditionEvent:
                    await ProcessRemoveRecordsByConditionEvent(removeRecordsByConditionEvent);
                    break;
                case CleanRecordsEvent<T> cleanRecordsEvent:
                    await ProcessCleanRecordsEvent(cleanRecordsEvent);
                    break;
            }
        }

        public async override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("OSKPROVIDER");
            var stream = streamProvider.GetStream<BasicEvent>(this.GetPrimaryKey(), "OSKNAMESPACE");
            await stream.SubscribeAsync(async (BasicEvent data, StreamSequenceToken token) => await Process(data));
        }

        public abstract Task ProcessCleanRecordsEvent(CleanRecordsEvent<T> @event);
        public abstract Task ProcessPutRecordsEvent(PutRecordsEvent<T> @event);
        public abstract Task ProcessRemoveRecordsByConditionEvent(RemoveRecordsByConditionEvent<T> @event);
        public abstract Task ProcessRemoveRecordsEvent(RemoveRecordsEvent<T> @event);
    }
}
