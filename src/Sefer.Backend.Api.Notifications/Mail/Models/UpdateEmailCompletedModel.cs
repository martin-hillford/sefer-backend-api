// This is the post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// A mail model for sending update email completed messages
/// </summary>
/// <inheritdoc />
public class UpdateEmailCompletedModel : UserMailModel
{
    /// <summary>
    /// The old e-mail of the user
    /// </summary>
    public string OldEmail { get; init; }

    /// <summary>
    /// The new e-mail of the user
    /// </summary>
    public string NewEmail { get; init; }

    /// <summary>
    /// A mail model for sending update email completed messages
    /// </summary>
    public UpdateEmailCompletedModel(MailData mailData) : base(mailData) { }
}