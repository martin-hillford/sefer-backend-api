// This is the post-model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// An abstract class to be used for all models that will contain information on a mail message to be sent
/// </summary>
public abstract class MailModel
{
    /// <summary>
    /// The e-mail options
    /// </summary>
    private readonly MailServiceOptions _options;

    /// <summary>
    /// Contains all the underlying meta-data
    /// </summary>
    protected readonly MailData Data;

    /// <summary>
    /// The site that is sending this e-mail
    /// </summary>
    public ISite Site => Data.Site;

    /// <summary>
    /// The site that is sending this e-mail
    /// </summary>
    public IRegion Region => Data.Region;

    /// <summary>
    /// The url of the client
    /// </summary>
    public string ClientUrl => Data.Site.SiteUrl;

    /// <summary>
    /// The email address of the admin
    /// </summary>
    public string AdminEmail => _options.AdminEmail;

    /// <summary>
    /// The short url for the site
    /// </summary>
    public string ShortUrl => Data.Site.Hostname;

    /// <summary>
    /// The language of the e-mail to the user
    /// </summary>
    public string Language => Data.Language;
    
    /// <summary>
    /// A generic mail model
    /// </summary>
    protected MailModel(MailData data)
    {
        _options = data.ServiceProvider.GetService<IOptions<MailServiceOptions>>()?.Value;
        Data = data;
    }
}