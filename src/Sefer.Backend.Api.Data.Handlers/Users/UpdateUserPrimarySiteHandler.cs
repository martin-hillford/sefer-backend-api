namespace Sefer.Backend.Api.Data.Handlers.Users;

public class UpdateUserPrimarySiteHandler(IServiceProvider serviceProvider) : Handler<UpdateUserPrimarySiteRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateUserPrimarySiteRequest request, CancellationToken token)
    {
        // Check if the user and the region / site exists
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        if (user == null) return false;

        var site = await Send(new GetSiteByNameRequest(request.Site.Hostname), token);
        var region = await Send(new GetRegionByIdRequest(request.Region.Id), token);
        if (region == null || site == null) return false;

        // If the user is mentor ensure that he is teaching on his new primary store

        if (user.IsMentor) await EnsureMentorStore(request, token);

        // update the primary store of the user
        user.PrimaryRegion = region.Id;
        user.PrimarySite = site.Hostname;

        var context = GetAsyncContext();
        return await context.UpdateAsync(user, token);
    }

    private async Task EnsureMentorStore(UpdateUserPrimarySiteRequest request, CancellationToken token)
    {
        var context = GetDataContext();

        var exists = await context.MentorRegions.AnyAsync(m => m.MentorId == request.UserId && m.RegionId == request.Region.Id, token);
        if (exists) return;

        var mentorStore = new MentorRegion { RegionId = request.Region.Id, MentorId = request.UserId };
        context.MentorRegions.Add(mentorStore);
        await context.SaveChangesAsync(token);
    }
}