namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateUserPrimarySiteRequest(int userId, ISite site, IRegion region) : IRequest<bool>
{
    public readonly int UserId = userId;

    public readonly ISite Site = site;

    public readonly IRegion Region = region;
}