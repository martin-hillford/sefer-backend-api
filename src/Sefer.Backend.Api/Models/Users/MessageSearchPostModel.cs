// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

/// <summary>
/// Searching for messages
/// </summary>
public class MessageSearchPostModel
{
    /// <summary>
    /// The term to search for
    /// </summary>
    [Required, MinLength(3)]
    public string Term { get; set; }
}