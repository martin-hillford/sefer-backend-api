// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Chat;

/// <summary>
/// Simple view for the count of the total unread messages
/// </summary>
public class CountView
{
    /// <summary>
    /// Number of unread message of the user
    /// </summary>
    public int Count { get; set; }
}