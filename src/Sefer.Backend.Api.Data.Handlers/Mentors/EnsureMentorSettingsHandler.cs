namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class EnsureMentorSettingsHandler(IServiceProvider serviceProvider)
    : Handler<EnsureMentorSettingsRequest, MentorSettings>(serviceProvider)
{
    public override async Task<MentorSettings> Handle(EnsureMentorSettingsRequest request, CancellationToken token)
    {
        var user = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (user?.IsMentor != true) return null;

        await using var context = GetDataContext();
        await EnsureMentorRegions(context, user);
        var mentorSettings = EnsureMentorSettings(context, user);
        return mentorSettings;
    }

    private static MentorSettings EnsureMentorSettings(DataContext context, User mentor)
    {
        var existing = context.MentorSettings.FirstOrDefault(ms => ms.MentorId == mentor.Id);
        if (existing != null) return existing;

        var mentorSettings = new MentorSettings
        {
            MaximumStudents = 0,
            PreferredStudents = 0,
            MentorId = mentor.Id,
        };

        context.MentorSettings.Add(mentorSettings);
        context.SaveChanges();
        return mentorSettings;
    }

    private async Task EnsureMentorRegions(DataContext context, User mentor)
    {
        var (region, _) = await Send(new GetPrimaryRegionAndSiteRequest(mentor.Id));

        var existing = context.MentorRegions.FirstOrDefault(ms => ms.MentorId == mentor.Id && ms.RegionId == region.Id);
        if (existing != null) return;

        var mentorRegion = new MentorRegion
        {
            MentorId = mentor.Id,
            RegionId = region.Id
        };

        context.MentorRegions.Add(mentorRegion);
        await context.SaveChangesAsync();
    }
}