using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;


namespace OrleansStatisticsKeeper.Client
{
    public static class StatisticsGrainExtension
    {
        public static async Task<ICollection<T>> Get<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk =>
            JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized())?.Where(x => func(x)).ToArray();

        public static async Task<T> GetFirst<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk 
            => JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized())?.FirstOrDefault(x => func(x));

        public static async Task<ICollection<T>> GetFirstN<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func, int elems)
            where T : DataChunk
            => JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized())?.Where(x => func(x)).Take(elems).ToArray();

        public static async Task<T> GetLast<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
            => JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized())?.Where(x => func(x)).Last();

        public static async Task<ICollection<T>> GetLastN<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func, int elems)
            where T : DataChunk
            => JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized()) ?.Where(x => func(x)).TakeLast(elems).ToArray();

        public static async Task<bool> AnyAsync<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
            => JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized()).Any(x => func(x));

        public static bool Any<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
        {
            var task = src.GetAllSerialized();
            task.Wait();

            return JsonConvert.DeserializeObject<ICollection<T>>(task.Result).Any(x => func(x));
        }

        public static async Task<IEnumerable<T>> Where<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
            => (JsonConvert.DeserializeObject<ICollection<T>>(await src.GetAllSerialized())).Where(x => func(x));
    }
}
