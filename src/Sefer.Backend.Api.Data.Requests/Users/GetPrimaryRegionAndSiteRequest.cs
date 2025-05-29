namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetPrimaryRegionAndSiteRequest(int userId) : IRequest<(IRegion, ISite)>
{
    public readonly int UserId = userId;
}