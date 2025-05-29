namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class IsSeriesNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsSeriesNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsSeriesNameUniqueRequest request, CancellationToken token)
    {
        var name = request.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return true;
        var context = GetDataContext();
        return !await context.Series
            .Where(sr => sr.Name.ToLower().Trim() == name && sr.Id != request.SeriesId)
            .AnyAsync(token);
    }
}