// This is post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// The model that will contain all required user information to generate a password forgot message
/// </summary>
/// <inheritdoc />
public class PasswordForgotModel : MailModel
{
    /// <summary>
    /// The user that requested a password forgot e-mail
    /// </summary>
    public readonly User User;

    /// <summary>
    /// Service which will help with generating hashes
    /// </summary>
    private readonly ICryptographyService _protection;

    /// <summary>
    /// Creates a new RegistrationComplete ViewModel
    /// </summary>
    public PasswordForgotModel(MailData mailData) : base(mailData)
    {
        _protection = mailData.ServiceProvider.GetService<ICryptographyService>();
        User = mailData.Receiver;
    }

    /// <summary>
    /// The reset link that can be used by the user to reset his password
    /// </summary>
    public string ResetLink
    {
        get
        {
            var queryString = _protection.TimeProtectedQueryString("u", User.Id.ToString());
            return Data.Site.SiteUrl + "/public/user/password-reset?" + queryString;
        }
    }
}