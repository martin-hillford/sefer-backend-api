namespace Sefer.Backend.Api.Data.Requests.Settings;

public class UpdateSettingsRequest(WebsiteSettings settings) : IRequest<bool>
{
    public readonly WebsiteSettings Settings = settings;
}