namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorSettingsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorSettingsRequest, MentorSettings>(serviceProvider)
{
    public override async Task<MentorSettings> Handle(GetMentorSettingsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.MentorSettings.FirstOrDefaultAsync(s => s.MentorId == request.MentorId, token);
    }
}