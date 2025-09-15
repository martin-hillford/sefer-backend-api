namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetEnrollmentsOfStudentHandler(IServiceProvider serviceProvider)
    : Handler<GetEnrollmentsOfStudentRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetEnrollmentsOfStudentRequest request, CancellationToken token)
    {
        if (request.Top < 1) return [];
        await using var context = GetDataContext();

        var query = context.Enrollments

            .Where(e => e.StudentId == request.UserId)
            .Include(e => e.CourseRevision).ThenInclude(c => c.Course);

        if (!request.Extensive)
        {
            return await query
                .AsNoTracking()
                .OrderByDescending(e => e.CreationDate)
                .Limit(request.Top)
                .ToListAsync(token);
        }

        await query.Include(e => e.CourseRevision).ThenInclude(c => c.Lessons).LoadAsync(token);
        await query.Include(e => e.LessonSubmissions).LoadAsync(token);
        await query.Include(e => e.Mentor).LoadAsync(token);
        return await query
            .OrderByDescending(e => e.CreationDate)
            .Limit(request.Top)
            .ToListAsync(token);

    }
}