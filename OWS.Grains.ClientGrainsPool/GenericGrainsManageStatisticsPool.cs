using OWS.Client;
using OWS.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OWS.Models;

namespace OWS.Grains.ClientGrainsPool
{
    public class GenericGrainsManageStatisticsPool<T> : GenericGrainsPool<IManageStatisticsGrain<T>>, IManageStatisticsGrain<T>
        where T : DataChunk
    {
        public GenericGrainsManageStatisticsPool(OrleansGrainsInnerClient client, int poolSize) : base(client, poolSize)
        {
        }

        public async Task<bool> Put(T obj)
            => await (await GetGrain()).Put(obj);

        public async Task<bool> Put(ICollection<T> objs)
        {
            try
            {
                foreach (var obj in objs)
                    await (await GetGrain()).Put(obj);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<long> Remove(Func<T, bool> func)
            => await (await GetGrain()).Remove(func);

        public async Task Remove(T obj)
            => await (await GetGrain()).Remove(obj);

        public async Task Remove(ICollection<T> objs)
        {
            foreach (var obj in objs)
                await (await GetGrain()).Remove(obj);
        }

        public async Task<long> Clean()
           => await (await GetGrain()).Clean();
    }
}
