namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetCoursesForSeriesHandler(IServiceProvider serviceProvider)
    : Handler<GetCoursesForSeriesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetCoursesForSeriesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.SeriesCourses
            .Where(sr => sr.SeriesId == request.SeriesId)
            .OrderBy(sr => sr.SequenceId)
            .Select(sr => sr.Course)
            .ToListAsync(token);
    }
}