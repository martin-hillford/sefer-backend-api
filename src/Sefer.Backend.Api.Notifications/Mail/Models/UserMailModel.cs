// This is post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// And empty model
/// </summary>
/// <inheritdoc />
public class UserMailModel : MailModel
{
    /// <summary>
    /// The user that requested a password forgot e-mail
    /// </summary>
    public readonly User User;

    /// <summary>
    /// A generic mail model
    /// </summary>
    public UserMailModel(MailData mailData) : base(mailData)
    {
        User = mailData.Receiver;
    }
}