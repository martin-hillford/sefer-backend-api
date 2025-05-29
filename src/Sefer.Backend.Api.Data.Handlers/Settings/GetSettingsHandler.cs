namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetSettingsHandler(IServiceProvider serviceProvider)
    : Handler<GetSettingsRequest, WebsiteSettings>(serviceProvider)
{
    public override async Task<WebsiteSettings> Handle(GetSettingsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var settings = await context.Settings.FirstOrDefaultAsync(token);
        if (settings != null) return settings;

        return new WebsiteSettings
        {
            Id = 0,
            RelativeAgeFactor = 0.25d,
            OptimalAgeDifference = 10,
            BackupMentorId = 0,
            SameMentorDays = 30,
            StudentActiveDays = 30,
            StudentReminderDays = 14,
            MaxLessonSubmissionsPerDay = 1
        };
    }
}