// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Extensions;

/// <summary>
/// Deals with extending the type object (useful for reflection!)
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets if this is an Enumerable type
    /// </summary>
    /// <param name="type"></param>
    /// <returns>True when Enumerable else false</returns>
    public static bool IsEnumerableType(this Type type)
    {
        return (type.GetInterface(nameof(IEnumerable)) != null);
    }

    /// <summary>
    /// Gets if this is a collection type
    /// </summary>
    /// <param name="type"></param>
    /// <returns>True when collection else false</returns>
    public static bool IsCollectionType(this Type type)
    {
        return (type.GetInterface(nameof(ICollection)) != null);
    }
}