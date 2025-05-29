namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class AddSeriesHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddSeriesRequest, Series>(serviceProvider)
{
    public override async Task<bool> Handle(AddSeriesRequest request, CancellationToken token)
    {
        if (request.Entity == null) return false;
        request.Entity.IsPublic = false;
        request.Entity.SequenceId = await GetHighestSequenceId(token) + 1;
        return await base.Handle(request, token);
    }

    private async Task<int> GetHighestSequenceId(CancellationToken token)
    {
        var context = GetDataContext();
        if (!await context.Series.AnyAsync(token)) return 0;
        return await context.Series.MaxAsync(c => c.SequenceId, token);
    }
}