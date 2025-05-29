namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class UpdateSettingsHandler(IServiceProvider serviceProvider)
    : Handler<UpdateSettingsRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateSettingsRequest request, CancellationToken token)
    {
        try
        {
            if (await IsValidAsync(request.Settings) == false) return false;

            var context = GetDataContext();
            var mentor = context.Users.SingleOrDefault(s => s.Id == request.Settings.BackupMentorId);
            if (mentor == null || mentor.IsMentor == false) return false;

            var dbSettings = context.Settings.FirstOrDefault();
            if (dbSettings == null)
            {
                request.Settings.Id = 0;
                context.Settings.Add(request.Settings);
            }
            else
            {
                dbSettings.BackupMentorId = request.Settings.BackupMentorId;
                dbSettings.OptimalAgeDifference = request.Settings.OptimalAgeDifference;
                dbSettings.RelativeAgeFactor = request.Settings.RelativeAgeFactor;
                dbSettings.SameMentorDays = request.Settings.SameMentorDays;
                dbSettings.StudentActiveDays = request.Settings.StudentActiveDays;
                dbSettings.StudentReminderDays = request.Settings.StudentReminderDays;
                dbSettings.MaxLessonSubmissionsPerDay = request.Settings.MaxLessonSubmissionsPerDay;
                context.Settings.Update(dbSettings);
            }

            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}