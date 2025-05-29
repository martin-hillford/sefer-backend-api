namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class AddRegionsForMentorHandler(IServiceProvider serviceProvider) : Handler<AddRegionsForMentorRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(AddRegionsForMentorRequest request, CancellationToken token)
    {
        if (request.Regions.Count == 0) return true;
        var user = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (user?.IsMentor != true) return false;

        var context = GetDataContext();
        var current = context.MentorRegions.Where(m => m.MentorId == request.MentorId);
        var lookup = await current.ToDictionaryAsync(m => m.RegionId, token);

        var adding = request.Regions
            .Where(region => !lookup.ContainsKey(region))
            .Select(region => new MentorRegion { RegionId = region, MentorId = request.MentorId })
            .ToList();

        if (adding.Count == 0) return true;

        context.AddRange(adding);
        await context.SaveChangesAsync(token);
        return true;
    }
}