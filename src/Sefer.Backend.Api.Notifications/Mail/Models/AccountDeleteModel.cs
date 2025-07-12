// This is post-model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// Model for an AccountDelete confirmation message to the user
/// </summary>
public class AccountDeleteModel : UserMailModel
{
    /// <summary>
    /// Service which will help with generating hashes
    /// </summary>
    private readonly ICryptographyService _protection;

    /// <summary>
    /// Creates a new PasswordReset
    /// </summary>
    /// <param name="mailData"></param>
    public AccountDeleteModel(MailData mailData) : base(mailData)
    {
        _protection = mailData.ServiceProvider.GetService<ICryptographyService>();
    }

    /// <summary>
    /// The delete link that can be used by the user to delete his account
    /// </summary>
    public string DeleteLink
    {
        get
        {
            var queryString = _protection.TimeProtectedQueryString("u", User.Id.ToString());
            return ClientUrl + "/public/confirm-delete-account?" + queryString;
        }
    }
}