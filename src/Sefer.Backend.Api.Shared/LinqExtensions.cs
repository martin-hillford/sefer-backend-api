using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sefer.Backend.Api.Shared;

public static class LinqExtensions
{
    public static bool NotContainsKey<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, TKey key)
        => !dictionary.ContainsKey(key);

    public static async Task<Dictionary<TKey, TSource>> ToDictionary<TSource, TKey>
    (
        this Task<List<TSource>> source,
        Func<TSource, TKey> keySelector
    ) 
        where TKey : notnull
    {
        var awaited = await source;
        return awaited.ToDictionary(keySelector);
    }
}