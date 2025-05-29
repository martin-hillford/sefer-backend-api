namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorPerformanceHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorPerformanceRequest, Dictionary<int, MentorPerformance>>(serviceProvider)
{
    public override async Task<Dictionary<int, MentorPerformance>> Handle(GetMentorPerformanceRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorPerformance.AsNoTracking().ToDictionaryAsync(p => p.MentorId, token);
    }
}