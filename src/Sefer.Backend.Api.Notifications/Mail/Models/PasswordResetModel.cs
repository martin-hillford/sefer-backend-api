// This is post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// Model for a Password Reset message to the user
/// </summary>
public class PasswordResetModel : MailModel
{
    /// <summary>
    /// The user that was registered
    /// </summary>
    public readonly User User;

    /// <summary>
    /// Creates a new PasswordReset
    /// </summary>
    public PasswordResetModel(MailData mailData) : base(mailData)
    {
        User = mailData.Receiver;
    }
}