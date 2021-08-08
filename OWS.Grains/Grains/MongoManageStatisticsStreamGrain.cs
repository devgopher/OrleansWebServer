using Orleans;
using OWS.Grains.Interfaces;
using OWS.Grains.StreamEvents;
using OWS.Grains.Utils;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoUtils = OWS.Grains.Utils.MongoUtils;
using System.Linq;
using OWS.Models;

namespace OWS.Grains.Grains
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
