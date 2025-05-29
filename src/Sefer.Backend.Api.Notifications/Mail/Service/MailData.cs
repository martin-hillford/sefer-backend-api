using System.Globalization;

namespace Sefer.Backend.Api.Notifications.Mail.Service;

public class MailData(IServiceProvider provider, User receiver, ISite site, IRegion region, string language)
{
    public readonly ISite Site = site;

    public readonly IRegion Region = region;

    public readonly string Language = language;

    public readonly User Receiver = receiver;

    public readonly IServiceProvider ServiceProvider = provider;
    
    // ReSharper disable once UnusedMember.Global
    public readonly string LogoUrl = site?.GetHeaderLogo(region);

    public CultureInfo GetCulture()
    {
        return Language switch
        {
            "nl" => CultureInfo.CreateSpecificCulture("nl-NL"),
            _ => null
        };
    }
}