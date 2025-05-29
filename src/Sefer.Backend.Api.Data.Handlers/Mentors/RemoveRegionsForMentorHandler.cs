namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class RemoveRegionsForMentorHandler(IServiceProvider serviceProvider) : Handler<RemoveRegionsForMentorRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(RemoveRegionsForMentorRequest request, CancellationToken token)
    {
        if (request.Regions.Count == 0) return true;
        var user = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (user?.IsMentor != true) return false;

        var context = GetDataContext();
        var current = await context.MentorRegions.Where(m => m.MentorId == request.MentorId).ToListAsync(token);

        var removing = current.Where(region => request.Regions.Contains(region.RegionId)).ToList();
        if (removing.Count == 0) return true;

        context.RemoveRange(removing);
        await context.SaveChangesAsync(token);
        return true;
    }
}