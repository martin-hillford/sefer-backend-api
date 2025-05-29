namespace Sefer.Backend.Api.Data.Requests.Settings;

public class GetRegionByIdRequest(string id) : IRequest<IRegion>
{
    public readonly string Id = id.ToLower();
}
