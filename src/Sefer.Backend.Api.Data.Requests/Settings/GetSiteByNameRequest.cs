namespace Sefer.Backend.Api.Data.Requests.Settings;

public class GetSiteByNameRequest(string name) : IRequest<ISite>
{
    public readonly string Name = name.ToLower();
}
