using System.Collections.Generic;

namespace Sefer.Backend.Api.Shared;

public static class LinqExtensions
{
    public static bool NotContainsKey<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, TKey key)
        => !dictionary.ContainsKey(key);
}