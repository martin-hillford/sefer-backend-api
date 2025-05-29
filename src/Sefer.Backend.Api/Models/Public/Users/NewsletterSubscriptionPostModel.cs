// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// Subscription of somebody to the newsletter
/// </summary>
public class NewsletterSubscriptionPostModel
{
    /// <summary>
    /// The name of the person that wants to subscribe for the newsletter
    /// </summary>
    [Required, MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// The e-mail of the person that wants to subscribe for the newsletter
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; }
}