namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorActiveStudentsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorActiveStudentsRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetMentorActiveStudentsRequest request, CancellationToken token)
    {
        var days = await GetDays(request);
        var activeDay = DateTime.UtcNow.AddDays(-1 * days);
        await using var context = GetDataContext();
        
        // Define a separate where clause, since include the lesson submissions gives issues
        var whereClause = context.EnrollmentSummaries
            .Join(context.Enrollments, s => s.Id, s => s.Id, (s, e) => new { Enrollment = e, Summary = s })
            .Where(j => j.Summary.Rank == 1 && j.Summary.StudentLastActive > activeDay && j.Summary.MentorId == request.MentorId);
            
        // Get the enrollments. Create a dictionary for performance when matching with the submission
        var enrollments = await whereClause
            .Select(a => a.Enrollment)
            .Include(e => e.Student)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .ToDictionaryAsync(l => l.Id, l => l, token);
        
        // Now get the submission separately and attach then to the enrollments
        var lessonSubmission = await whereClause
            .Join(context.LessonSubmissions, s => s.Summary.Id, l => l.EnrollmentId, (s, l) => l)
            .ToListAsync(token);
        foreach (var lesson in lessonSubmission) { enrollments[lesson.EnrollmentId].LessonSubmissions.Add(lesson); }

        // And return the result
        return enrollments.Values.ToList();
    }

    private async Task<short> GetDays(GetMentorActiveStudentsRequest request)
    {
        if (request.ActiveDays != null) return request.ActiveDays.Value;
        var settings = await Send(new GetSettingsRequest());
        return settings?.StudentActiveDays ?? 30;
    }
}