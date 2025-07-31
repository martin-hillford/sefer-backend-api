// This is the post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// A mail model for sending update e-mail requests
/// </summary>
public class UpdateEmailRequestModel : UserMailModel
{
    /// <summary>
    /// Contains the information of email data (that is encrypted data)
    /// </summary>
    public string EmailChangeData { get; init; }

    /// <summary>
    /// The new e-mail address of the user
    /// </summary>
    public string NewMail { get; init; }

    /// <summary>
    /// Holds the e-mail to change the address
    /// </summary>
    public string EmailLink
    {
        get
        {
            var queryString = _protection.TimeProtectedQueryString("c", EmailChangeData);
            return Data.Site.SiteUrl + "/public/user/update-mail?m=" + WebUtility.UrlEncode(NewMail) + "&" + queryString;
        }
    }

    /// <summary>
    /// Service which will help with generating hashes
    /// </summary>
    private readonly ICryptographyService _protection;

    /// <summary>
    /// A mail model for sending change e-mail methods
    /// </summary>
    public UpdateEmailRequestModel(MailData mailData) : base(mailData)
    {
        _protection = mailData.ServiceProvider.GetService<ICryptographyService>();
    }
}