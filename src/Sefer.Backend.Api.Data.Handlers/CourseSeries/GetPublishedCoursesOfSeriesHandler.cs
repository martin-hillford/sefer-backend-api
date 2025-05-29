namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetPublishedCoursesOfSeriesHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesOfSeriesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetPublishedCoursesOfSeriesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var series = await context.Series.SingleOrDefaultAsync(s => s.Id == request.SeriesId, token);
        if (series == null) return new List<Course>();

        return await context.SeriesCourses
            .Where(sr => sr.SeriesId == series.Id && sr.Course.CourseRevisions.Any(r => r.Stage == Stages.Published))
            .OrderBy(sr => sr.SequenceId)
            .Select(sr => sr.Course)
            .ToListAsync(token);
    }
}