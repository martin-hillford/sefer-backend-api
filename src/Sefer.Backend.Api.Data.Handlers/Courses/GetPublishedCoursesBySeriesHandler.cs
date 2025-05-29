namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCoursesBySeriesHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesBySeriesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetPublishedCoursesBySeriesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.SeriesCourses
            .Where(s => s.SeriesId == request.SeriesId &&
                        s.Course.CourseRevisions.Any(r => r.Stage == Stages.Published))
            .OrderBy(s => s.SequenceId)
            .Select(s => s.Course)
            .ToListAsync(token);
    }
}