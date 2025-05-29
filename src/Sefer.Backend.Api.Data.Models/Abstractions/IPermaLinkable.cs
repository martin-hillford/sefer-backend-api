namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// IPermaLinkable is implemented by objects that are 'exposed' using a unique url.
/// </summary>
/// <example>A Course will be display on page</example>
/// <inheritdoc cref="IEntity"/>
public interface IPermaLinkable : IEntity
{
    /// <summary>
    /// Gets the name for the object.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the permalink for the object.
    /// </summary>
    string Permalink { get; set; }
}