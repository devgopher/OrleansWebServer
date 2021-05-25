using MongoDB.Driver;
using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;
using MongoUtils = OrleansStatisticsKeeper.Grains.Utils.MongoUtils;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class MongoManageStatisticsGrain<T> : Grain, IManageStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;

        public MongoManageStatisticsGrain(MongoUtils mongoUtils) => _mongoUtils = mongoUtils;

        public async Task<long> Clean()
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteManyAsync(d => true);

                return delResult.DeletedCount;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> Put(T obj)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                await collection.InsertOneAsync(obj);
            }
            catch
            {
                return false;
            } 

            return true;
        }

        public async Task<bool> Put(ICollection<T> objs)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                await collection.InsertManyAsync(objs);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<long> Remove(Func<T, bool> func)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteManyAsync(f => func(f));

                return delResult.DeletedCount;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task Remove(T obj)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                var delResult = await collection.DeleteOneAsync(t => t.Id == obj.Id);
            }
            catch
            {
            }
        }

        public async Task Remove(ICollection<T> objs)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            var delResult = await collection.DeleteManyAsync(t => objs.Contains(t));
        }
    }
}
