namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetEnrollmentsOfStudentHandler(IServiceProvider serviceProvider)
    : Handler<GetEnrollmentsOfStudentRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetEnrollmentsOfStudentRequest request, CancellationToken token)
    {
        if (request.Top < 1) return [];
        await using var context = GetDataContext();

        var query = context.Enrollments.Where(e => e.StudentId == request.UserId);
        if (request.Start.HasValue)
        {
            query = query.Where(e =>
                e.CreationDate >= request.Start ||
                e.ModificationDate >= request.Start ||
                e.ClosureDate >= request.Start
            );
        }
        
        var include = query.Include(e => e.CourseRevision).ThenInclude(c => c.Course);

        if (!request.Extensive)
        {
            return await include
                .AsNoTracking()
                .OrderByDescending(e => e.CreationDate)
                .Limit(request.Top)
                .ToListAsync(token);
        }

        await include.Include(e => e.CourseRevision).ThenInclude(c => c.Lessons).LoadAsync(token);
        await include.Include(e => e.LessonSubmissions).LoadAsync(token);
        await include.Include(e => e.Mentor).LoadAsync(token);
        return await include
            .OrderByDescending(e => e.CreationDate)
            .Limit(request.Top)
            .ToListAsync(token);
    }
}