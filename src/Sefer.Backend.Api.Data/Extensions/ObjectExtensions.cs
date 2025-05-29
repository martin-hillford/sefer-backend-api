namespace Sefer.Backend.Api.Data.Extensions;

public static class ObjectExtensions
{
    public static bool ContainsRegion(this ISite site, IRegion region)
    {
        if (site == null || region == null) return false;
        return site.Type switch
        {
            SiteType.Dynamic => true,
            SiteType.Static => site.RegionId == region.Id,
            _ => false
        };
    }
}