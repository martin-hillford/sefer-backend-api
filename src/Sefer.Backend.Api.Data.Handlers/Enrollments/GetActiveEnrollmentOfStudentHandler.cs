namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetActiveEnrollmentOfStudentHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveEnrollmentOfStudentRequest, Enrollment>(serviceProvider)
{
    public override async Task<Enrollment> Handle(GetActiveEnrollmentOfStudentRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var query = context.Enrollments
            .AsNoTracking()
            .Where(s => s.StudentId == request.StudentId && s.ClosureDate == null)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(e => e.Mentor);

        if (!request.Extensively) return await query.FirstOrDefaultAsync(token);

        return await query
            .Include(e => e.LessonSubmissions)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Lessons)
            .FirstOrDefaultAsync(token);
    }
}