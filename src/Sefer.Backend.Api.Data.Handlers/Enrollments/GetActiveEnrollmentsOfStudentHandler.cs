namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetActiveEnrollmentsOfStudentHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetActiveEnrollmentsOfStudentRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var query = context.Enrollments
            .AsNoTracking()
            .Where(s => s.StudentId == request.StudentId && s.ClosureDate == null)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(e => e.Mentor);

        if (!request.Extensively) return await query.ToListAsync(token);

        return await query
            .Include(e => e.LessonSubmissions)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Lessons)
            .ToListAsync(token);
    }
}