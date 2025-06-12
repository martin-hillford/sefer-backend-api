namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetActiveStudentsOfMentorsHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveStudentsOfMentorsRequest, MentorActiveStudentsDictionary>(serviceProvider)
{
    public override async Task<MentorActiveStudentsDictionary> Handle(GetActiveStudentsOfMentorsRequest request, CancellationToken token)
    {
        var days = await GetDays();
        if (days == null) return null;

        var context = GetDataContext();
        var activeDate = DateTime.UtcNow.AddDays(-1 * days.Value);
        
        if (context.Database.IsSqlCapableServer())
        {
            var activeUsingSql = await context.Set<ActiveStudentsPerMentor>().ToDictionaryAsync(g => g.MentorId, g => g.ActiveStudents, token);
            return new MentorActiveStudentsDictionary(activeUsingSql);
        }

        var active = await context.UserLastActivities
            .Join(context.Enrollments, a => a.UserId, e => e.StudentId, (a, e) => new { e.MentorId, e.StudentId, a.ActivityDate, e.ClosureDate })
            .Where(e => e.ClosureDate == null && e.MentorId != null && e.ActivityDate >= activeDate)
            .GroupBy(g => g.MentorId.Value)
            .ToDictionaryAsync(grp => grp.Key, grp => grp.Count(), token);

        return new MentorActiveStudentsDictionary(active);
    }

    private async Task<short?> GetDays()
    {
        var settings = await Send(new GetSettingsRequest());
        return settings?.StudentActiveDays;
    }
}