namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorRegionsHandler(IServiceProvider serviceProvider) : Handler<GetMentorRegionsRequest, List<string>>(serviceProvider)
{
    public override async Task<List<string>> Handle(GetMentorRegionsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorRegions
            .AsNoTracking()
            .Where(m => m.MentorId == request.MentorId)
            .Select(m => m.RegionId)
            .ToListAsync(token);
    }
}