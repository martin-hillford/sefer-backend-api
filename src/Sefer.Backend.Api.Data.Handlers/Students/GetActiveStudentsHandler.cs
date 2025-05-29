namespace Sefer.Backend.Api.Data.Handlers.Students;

public class GetActiveStudentsHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveStudentsRequest, HashSet<int>>(serviceProvider)
{
    public override async Task<HashSet<int>> Handle(GetActiveStudentsRequest request, CancellationToken token)
    {
        var settings = await Send(new GetSettingsRequest(), token);
        var date = DateTime.UtcNow.Date.AddDays(-1 * settings.StudentActiveDays);

        await using var context = GetDataContext();
        return context.UserLastActivities
            .AsNoTracking()
            .Where(e => e.ActivityDate.Date >= date)
            .Select(e => e.UserId)
            .ToHashSet();
    }
}