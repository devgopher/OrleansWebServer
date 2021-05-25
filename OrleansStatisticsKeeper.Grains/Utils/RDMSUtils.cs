using DapperExtensions;
using OrleansStatisticsKeeper.Grains.Database;
using OrleansStatisticsKeeper.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains.Utils
{
    public class RDMSUtils
    {
        private readonly OskSettings _settings;

        public RDMSUtils(OskSettings settings) => _settings = settings;

        public async Task<IEnumerable<T>> GetData<T>() where T : DataChunk
        {
            using var rdmsConnection = ConnectionsFactory.OpenRdms(_settings.ConnectionType, _settings.ConnectionString);
            return await rdmsConnection.GetListAsync<T>();
        }
    }
}
