using Sefer.Backend.Api.Data.Requests.CourseSeries;

namespace Sefer.Backend.Api.Controllers.Public;

public class SeriesController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    [HttpGet("/public/series")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<List<SeriesListView>>> GetPublishedSeries()
    {
        var series = await Send(new GetPublicSeriesRequest());
        var view = series.Select(s => new SeriesListView(s)).ToList();
        return Json(view);
    }

    [HttpGet("/public/series/{id:int}")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<List<SeriesWithCourseSummaryView>>> GetSeries(int id)
    {
        var series = await Send(new GetSeriesByIdRequest(id));
        if (series == null || series.IsPublic == false) return NotFound();

        var courses = await Send(new GetPublishedCoursesBySeriesRequest(series.Id));
        var counts = await Send(new GetStudentsCountPerCourseRequest());
        var readingTime = await Send(new GetCourseReadingTimeRequest());

        var fileStorageService = GetService<IFileStorageService>();
        var coursesView = courses.Select(c => new CourseSummaryView(c, readingTime[c.Id], fileStorageService, counts[c.Id])).ToList();
        var view = new SeriesWithCourseSummaryView(series) { Courses = coursesView };
        return Json(view);
    }
}