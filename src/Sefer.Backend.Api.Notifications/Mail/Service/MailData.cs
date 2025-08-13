using System.Globalization;
using System.Text.Json.Serialization;

namespace Sefer.Backend.Api.Notifications.Mail.Service;

public class MailData(IServiceProvider provider, User receiver, ISite site, IRegion region, string language)
{
    [JsonIgnore]
    public User User => receiver;

    [JsonIgnore]
    public readonly IServiceProvider ServiceProvider = provider;
    
    public ISite Site => site;

    public IRegion Region => region;

    public string Language => language;

    public UserView Receiver => new(receiver);
    
    // ReSharper disable once UnusedMember.Global
    public string LogoUrl => site?.GetHeaderLogo(region);

    public CultureInfo GetCulture()
    {
        return Language switch
        {
            "nl" => CultureInfo.CreateSpecificCulture("nl-NL"),
            "en" => CultureInfo.CreateSpecificCulture("en-GB"),
            _ => null
        };
    }
}