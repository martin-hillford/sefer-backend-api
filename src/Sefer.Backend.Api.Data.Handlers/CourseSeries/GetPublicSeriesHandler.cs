namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetPublicSeriesHandler(IServiceProvider serviceProvider)
    : Handler<GetPublicSeriesRequest, List<Series>>(serviceProvider)
{
    public override async Task<List<Series>> Handle(GetPublicSeriesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Series
            .Where(s => s.IsPublic)
            .Include(s => s.SeriesCourses)
            .OrderBy(s => s.SequenceId)
            .ToListAsync(token);
    }
}