namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class GetSeriesHandler(IServiceProvider serviceProvider) : Handler<GetSeriesRequest, List<Series>>(serviceProvider)
{
    public override async Task<List<Series>> Handle(GetSeriesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Series.OrderBy(s => s.SequenceId).AsNoTracking().ToListAsync(token);
    }
}
