namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorActiveStudentsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorActiveStudentsRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetMentorActiveStudentsRequest request, CancellationToken token)
    {
        var days = await GetDays(request);
        var activeDay = DateTime.UtcNow.AddDays(-1 * days);
        await using var context = GetDataContext();
        return await context.EnrollmentSummaries
            .Join(context.Enrollments, s => s.Id, s => s.Id, (s, e) => new { Enrollment = e, Summary = s })
            .Where(j => j.Summary.Rank == 1 && j.Summary.StudentLastActive > activeDay && j.Summary.MentorId == request.MentorId)
            .Select(a => a.Enrollment)
            .Include(e => e.Student)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .ToListAsync(token);
    }

    private async Task<short> GetDays(GetMentorActiveStudentsRequest request)
    {
        if (request.ActiveDays != null) return request.ActiveDays.Value;
        var settings = await Send(new GetSettingsRequest());
        return settings?.StudentActiveDays ?? 30;
    }
}