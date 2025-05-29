namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetTakenCoursesOfStudentHandler(IServiceProvider serviceProvider)
    : Handler<GetTakenCoursesOfStudentRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetTakenCoursesOfStudentRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == request.StudentId)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course)
            .Include(e => e.LessonSubmissions)
            .OrderByDescending(e => e.CreationDate)
            .ToListAsync(token);
    }
}