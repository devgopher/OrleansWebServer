using MongoDB.Driver;
using OrleansStatisticsKeeper.Grains.Database;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;
using OrleansStatisticsKeeper.Models.Settings;

namespace OrleansStatisticsKeeper.Grains.Utils
{
    public class MongoUtils
    {
        private readonly OskSettings _settings;

        public MongoUtils(OskSettings settings) => _settings = settings;

        public async Task<IMongoCollection<T>> GetCollection<T>()
            where T : DataChunk
        {
            var typeName = typeof(T).Name;
            var mongoConnection = ConnectionsFactory.OpenMongo(_settings.ConnectionString);
            var database = mongoConnection.GetDatabase(_settings.Database);
            IMongoCollection<T> collection;

            if ((await database.ListCollectionNamesAsync()).ToEnumerable().All(c => c != typeName))
            {
                await database.CreateCollectionAsync(typeName);
                collection = database.GetCollection<T>(typeName);
                var indexKeys = Builders<T>.IndexKeys.Ascending(t => t.Id)
                    .Descending(t => t.DateTimeTicks);

                await collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeys));
            } else
                collection = database.GetCollection<T>(typeName);
            
            return collection;
        }
    }
}
