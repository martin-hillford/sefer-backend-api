namespace Sefer.Backend.Api.Data.Handlers.Students;

public class GetStudentInformationHandler(IServiceProvider serviceProvider) : Handler<GetStudentInformationRequest, (User, EnrollmentSummary, bool)?>(serviceProvider)
{
    public override async Task<(User, EnrollmentSummary, bool)?> Handle(GetStudentInformationRequest request, CancellationToken token)
    {
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student == null) return null;

        await using var context = GetDataContext();
        var current = await context.EnrollmentSummaries.SingleOrDefaultAsync(s => s.StudentId == request.StudentId && s.Rank == 1, token);
        if (current == null) return null;

        var query = context.Enrollments.AsNoTracking().Where(u => u.StudentId == request.StudentId);
        var courses = await query
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .ToLookupAsync(e => e.Id, token);

        var lessons = await query
            .Include(e => e.CourseRevision).ThenInclude(c => c.Lessons)
            .ToLookupAsync(e => e.Id, token);
        var submissions = await query
            .Include(e => e.LessonSubmissions)
            .ToLookupAsync(e => e.Id, token);
        var mentors = await query
            .Include(e => e.Mentor)
            .ToLookupAsync(e => e.Id, token);
        var enrollments = await query.Include(e => e.CourseRevision).ToListAsync(token);

        foreach (var enrollment in enrollments)
        {
            enrollment.CourseRevision.Course = courses[enrollment.Id].CourseRevision.Course;
            enrollment.CourseRevision.Lessons = lessons[enrollment.Id].CourseRevision.Lessons;
            enrollment.LessonSubmissions = submissions[enrollment.Id].LessonSubmissions;
            enrollment.Mentor = mentors[enrollment.Id].Mentor;
        }

        var settings = await Send(new GetSettingsRequest(), token);
        var activeDate = DateTime.UtcNow.AddDays(-settings.StudentActiveDays);
        var lastActive = await context.UserLastActivities.SingleOrDefaultAsync(l => l.UserId == student.Id, token);
        var active = lastActive.ActivityDate >= activeDate;

        student.Enrollments = enrollments;
        return (student, current, active);
    }
}