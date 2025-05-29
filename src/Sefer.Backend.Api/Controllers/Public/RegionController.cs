using Sefer.Backend.GeoIP.Lib;
using GeoIPInfo = Sefer.Backend.GeoIP.Lib.GeoIPInfo;

namespace Sefer.Backend.Api.Controllers.Public;

/// <summary>
/// This controller deals with region related requests from the front-end
/// </summary>
/// <param name="serviceProvider"></param>
public class RegionController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [ResponseCache(Duration = 60)]
    [HttpGet("/get-region")]
    public async Task<ActionResult<RequestRegionView>> GetRegion()
    {
        var regions = await Send(new GetRegionRequest());
        var geoInfo = await GetGeoIPAsync();

        var selected = GetRegionForGeoInfo(regions, geoInfo);

        return new RequestRegionView(selected, regions);
    }

    private async Task<GeoIPInfo> GetGeoIPAsync()
    {
        var geoIPService = ServiceProvider.GetService<IGeoIPService>();
        var context = ServiceProvider.GetService<IHttpContextAccessor>();
        var ipAddress = context.GetClientIpAddress();
        return await geoIPService.GetInfo(ipAddress);
    }

    private static IRegion GetRegionForGeoInfo(List<IRegion> regions, GeoIPInfo geoInfo)
    {
        return geoInfo?.CountryCode?.ToLower() switch
        {
            "sr" => GetUserRegion(regions, "sr"),
            "be" => GetUserRegion(regions, "be"),
            _ => GetDefaultRegion(regions)
        };
    }

    private static IRegion GetUserRegion(List<IRegion> regions, string code)
    {
        var region = regions.FirstOrDefault(s => s.Id.Equals(code, StringComparison.CurrentCultureIgnoreCase));
        return region ?? GetDefaultRegion(regions);
    }

    private static IRegion GetDefaultRegion(List<IRegion> regions)
    {
        var defaultRegion = regions.FirstOrDefault(s => s.Id.Equals("nl", StringComparison.CurrentCultureIgnoreCase));
        return defaultRegion ?? regions.First();
    }
}