using Sefer.Backend.Api.Models.Public.Home;
using Sefer.Backend.Api.Views.Public.Resources;
using Sefer.Backend.Api.Views.Shared.Courses;
using Sefer.Backend.GeoIP.Lib;

namespace Sefer.Backend.Api.Controllers.Public;

public class HomeController(IServiceProvider provider) : BaseController(provider)
{
    private readonly IGeoIPService _geoIpService = provider.GetGeoIPService();

    private readonly IHttpContextAccessor _contextAccessor = provider.GetService<IHttpContextAccessor>();

    [HttpGet("/courses/homepage")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(List<CourseDisplayView>), 200)]
    public async Task<IActionResult> GetHomepageCourses()
    {
        var courses = await Send(new GetHomepageCoursesRequest());
        var studentCounts = await Send(new GetStudentsCountPerCourseRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());
        var fileStorageService = GetService<IFileStorageService>();
        var view = courses.Select(c => new CourseSummaryView(c, readingTime[c.Id], fileStorageService, studentCounts[c.Id]));
        return Json(view);
    }

    [HttpGet("/testimonies/homepage")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(List<TestimonyView>), 200)]
    public async Task<IActionResult> GetHomepageTestimonies()
    {
        var testimonies = await Send(new GetRandomTestimoniesRequest(3, true));
        var view = testimonies.Select(c => new TestimonyView(c));
        return Json(view);
    }

    [HttpGet("/blogs/homepage")]
    [ProducesResponseType(typeof(List<BlogBaseView>), 200)]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<List<BlogBaseView>>> GetHomepageBlogs()
    {
        var blogs = await Send(new GetPublishedBlogsRequest(2));
        return Json(blogs.Select(b => new BlogBaseView(b)));
    }

    [HttpPost("/log/page")]
    public async Task<ActionResult> LogClientPage([FromBody] PageLogPostModel pageLog)
    {
        try
        {
            var logEntry = await _contextAccessor.GetLogEntry(pageLog, _geoIpService);
            await Send(new AddClientPageRequestLogEntryRequest(logEntry));
            return StatusCode(200);
        }
        catch (Exception)
        {
            return StatusCode(200);
        }
    }

    [HttpGet("/public/pages/list")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<List<ContentPageLink>>> GetPublishedMenuPagesLinks()
    {
        var links = await Send(new GetPublishedMenuPagesLinksRequest());
        return Json(links);
    }

    [HttpGet("/clear-site-data")]
    public ActionResult ClearSideData()
    {
        var httpContext = ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        httpContext?.Response.Headers.Append("Clear-Site-Data", "\"*\"");
        return Redirect("/cache-cleared");
    }

    [HttpGet("/geo-info")]
    public async Task<ActionResult> GetGeoInfo()
    {
        var ipAddress = _contextAccessor.GetClientIpAddress();
        if (string.IsNullOrEmpty(ipAddress)) return BadRequest();
        var geoInfo = await _geoIpService.GetInfo(ipAddress);
        return Json(new { geoInfo, ipAddress });
    }
}