namespace Sefer.Backend.Api.Data.Models.Extensions;

/// <summary>
/// Extensions methods
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Return is this object is of a numeric type
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public static string GeneratePermalink(this IPermaLinkable o)
    {
        if (string.IsNullOrEmpty(o.Name)) return o.Name;
        var proto = o.Name.Trim().ToLower().TrimSpaces().Replace(' ', '-');
        return Regex.Replace(proto, "[^-a-z]", string.Empty);
    }
}