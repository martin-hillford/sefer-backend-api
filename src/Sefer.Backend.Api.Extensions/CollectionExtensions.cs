// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

/// <summary>
/// These extensions provide additional methods for lists and other collection
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Returns the list ordered (ascending)
    /// </summary>
    /// <typeparam name="T">The type of the list</typeparam>
    /// <typeparam name="TKey">The type of the object to order the list on</typeparam>
    /// <param name="list">The list to order</param>
    /// <param name="selector">A selector to use</param>
    /// <returns></returns>
    public static List<T> ToOrderedList<T, TKey>(this List<T> list, Func<T, TKey> selector)
    {
        return list.OrderBy(selector).ToList();
    }

    /// <summary>
    /// Returns count number of random elements from queryable
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="queryable">A queryable to select the elements from</param>
    /// <param name="count">The number of elements to select. If count exceed the number of elements in queryable, all elements are returned</param>
    /// <returns>A queryable with count elements randomized</returns>
    public static IQueryable<T> RandomElements<T>(this IQueryable<T> queryable, int count)
    {
        return queryable.OrderBy(s => Guid.NewGuid().ToString()).Take(count);
    }

    /// <summary>
    /// Returns count number of random elements from queryable
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="queryable">A queryable to select the elements from</param>
    /// <param name="expression">A where expression to apply on the queryable before selecting the random elements</param>
    /// <param name="count">The number of elements to select. If count exceed the number of elements in queryable, all elements are returned</param>
    /// <returns>A queryable with count elements randomized</returns>
    public static IQueryable<T> RandomElements<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> expression, int count)
    {
        return queryable.Where(expression).OrderBy(s => Guid.NewGuid().ToString()).Take(count);
    }

    /// <summary>
    /// Returns a random element from the queryable
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="queryable">A queryable to select the element from</param>
    /// <param name="expression">A where expression to apply on the queryable before selecting the random element</param>
    /// <returns>A random selected element</returns>
    public static T RandomElement<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> expression)
    {
        var r = new Random();
        queryable = queryable.Where(expression);
        return queryable.Skip(r.Next(queryable.Count())).FirstOrDefault();
    }

    /// <summary>
    /// Returns a random element from the queryable
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="queryable">A queryable to select the element from</param>
    /// <returns>A random selected element</returns>
    public static T RandomElement<T>(this IQueryable<T> queryable)
    {
        var r = new Random();
        return queryable.Skip(r.Next(queryable.Count())).FirstOrDefault();
    }

    /// <summary>
    /// Returns a random element from the queryable
    /// </summary>
    /// <typeparam name="T">The type of the elements</typeparam>
    /// <param name="queryable">A queryable to select the element from</param>
    /// <returns>A random selected element</returns>
    public static T RandomElement<T>(this List<T> queryable)
    {
        var r = new Random();
        return queryable.Skip(r.Next(queryable.Count)).FirstOrDefault();
    }

    public static List<T> ToListThenForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        var list = enumerable.ToList();
        list.ForEach(action);
        return list;
    }

    public static async Task<List<T>> ToListThenForEachAsync<T>(this IQueryable<T> enumerable, Action<T> action, CancellationToken token = default)
    {
        var list = await enumerable.ToListAsync(token);
        list.ForEach(action);
        return list;
    }

    public static IQueryable<TSource> Limit<TSource>(this IQueryable<TSource> queryable, int? take)
    {
        return take == null ? queryable : queryable.Take(take.Value);
    }

    public static async Task<IDictionary<TKey, TSource>> ToLookupAsync<TSource, TKey>(
        this IQueryable<TSource> source,
        Func<TSource, TKey> keySelector,
        CancellationToken cancellationToken = default)
        where TKey : notnull
    {
        return await source.ToDictionaryAsync(keySelector, c => c, cancellationToken);
    }

    public static Dictionary<TKey, TSource> Merge<TKey, TSource>(this Dictionary<TKey, TSource> source, IDictionary<TKey, TSource> other)
    {
        foreach (var pair in other)
        {
            if(!source.ContainsKey(pair.Key)) source.Add(pair.Key, pair.Value);
        }
        return source;
    }

    public static TValue GetValuesOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
    {
        return !dictionary.TryGetValue(key, out var value) ? defaultValue : value;
    }
}