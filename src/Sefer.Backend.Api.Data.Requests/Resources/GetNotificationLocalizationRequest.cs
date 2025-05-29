namespace Sefer.Backend.Api.Data.Requests.Resources;

public class GetNotificationLocalizationRequest(string name, string language) : IRequest<NotificationLocalization>
{
    public readonly string Name = name;
    
    public readonly string Language = language;
}