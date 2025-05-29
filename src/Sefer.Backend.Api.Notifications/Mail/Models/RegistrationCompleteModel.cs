// This is post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global

using Sefer.Backend.Api.Services.Extensions;

namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// RegistrationComplete is a ViewModel for the registration e-mail to be sent
/// </summary>
/// <inheritdoc />
/// <remarks>
/// Creates a new RegistrationComplete ViewModel
/// </remarks>
public class RegistrationCompleteModel(MailData mailData) : MailModel(mailData)
{
    /// <summary>
    /// Service which will help with generating hashes
    /// </summary>
    private readonly ICryptographyService _protection = mailData.ServiceProvider.GetService<ICryptographyService>();

    /// <summary>
    /// The user that was registered
    /// </summary>
    public readonly User User = mailData.Receiver;

    /// <summary>
    /// The activation link that can be used by the user to activate his account
    /// </summary>
    public string ActivationLink
    {
        get
        {
            var queryString = _protection.ProtectedQueryString("u", User.Id.ToString());
            return ClientUrl + "/public/user/activate?" + queryString;
        }
    }

    /// <summary>
    /// The activation link that can be used by the user to activate his account
    /// </summary>
    public string ActivationCode => _protection.GetUserActivationCode(User);
}