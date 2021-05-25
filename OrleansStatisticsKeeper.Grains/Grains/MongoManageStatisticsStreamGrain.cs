using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.StreamEvents;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoUtils = OrleansStatisticsKeeper.Grains.Utils.MongoUtils;
using System.Linq;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    [ImplicitStreamSubscription("OSKNAMESPACE")]
    public class MongoManageStatisticsStreamGrain<T> : GenericManageStatisticsStreamGrain<T>, IManageStatisticsStreamGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;

        public MongoManageStatisticsStreamGrain(MongoUtils mongoUtils) => _mongoUtils = mongoUtils;

        public override async Task ProcessCleanRecordsEvent(CleanRecordsEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteManyAsync(d => true);
            }
            catch (Exception)
            {
            }
        }

        public override Task ProcessPutRecordsEvent(PutRecordsEvent<T> @event)
        {
            throw new System.NotImplementedException();
        }

        public override async Task ProcessRemoveRecordsByConditionEvent(RemoveRecordsByConditionEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteManyAsync(f => @event.ConditionFunc(f));
            }
            catch (Exception)
            {
            }
        }

        public override async Task ProcessRemoveRecordsEvent(RemoveRecordsEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteManyAsync(f => @event.Data.Any(d => f.Id == d.Id));
            }
            catch (Exception)
            {
            }
        }
    }
}
