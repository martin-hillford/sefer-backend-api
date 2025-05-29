namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetStudentActivityHandler(IServiceProvider serviceProvider)
    : Handler<GetStudentActivityRequest, HashSet<int>>(serviceProvider)
{
    public override async Task<HashSet<int>> Handle(GetStudentActivityRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var students = context.Enrollments.Where(e => e.MentorId == request.MentorId).Select(e => e.StudentId);
        var activity = await context.UserLastActivities.Where(e => students.Contains(e.UserId)).ToListAsync(token);

        var settings = await Send(new GetSettingsRequest(), token);
        var active = DateTime.UtcNow.AddDays(-settings.StudentActiveDays);

        return activity.Where(l => l.ActivityDate >= active).Select(l => l.UserId).ToHashSet();
    }
}