using DapperExtensions;
using OWS.Grains.Database;
using OWS.Models.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWS.Grains.Models;
using OWS.Models;

namespace OWS.Grains.Utils
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
