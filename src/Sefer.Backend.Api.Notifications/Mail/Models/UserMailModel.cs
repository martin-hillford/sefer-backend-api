// This is the post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// And empty model
/// </summary>
/// <inheritdoc />
public class UserMailModel(MailData mailData) : MailModel(mailData)
{
    /// <summary>
    /// The user that requested a password forgot e-mail
    /// </summary>
    public UserView User => Data.Receiver;
}