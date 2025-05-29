namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetEnrollmentByIdExtensivelyHandler(IServiceProvider serviceProvider)
    : Handler<GetEnrollmentByIdExtensivelyRequest, Enrollment>(serviceProvider)
{
    public override async Task<Enrollment> Handle(GetEnrollmentByIdExtensivelyRequest request, CancellationToken token)
    {
        if (request.EnrollmentId == null) return null;
        var context = GetDataContext();
        var query = context.Enrollments.Where(e => e.Id == request.EnrollmentId);

        // EF gets slow with many includes, so load then in blocks
        await query.Include(e => e.CourseRevision).ThenInclude(c => c.Course).LoadAsync(token);
        await query.Include(e => e.Mentor).LoadAsync(token);
        await query.Include(e => e.LessonSubmissions).LoadAsync(token);
        await query.Include(e => e.CourseRevision).ThenInclude(c => c.Lessons).LoadAsync(token);
        await query.Include(e => e.LessonSubmissions).ThenInclude(l => l.Lesson).LoadAsync(token);
        await query.Include(e => e.LessonSubmissions).ThenInclude(l => l.Answers).LoadAsync(token);
        await query.Include(e => e.Student).LoadAsync(token);

        return await query.SingleOrDefaultAsync(token);
    }
}