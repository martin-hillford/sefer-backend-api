namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetCompletedCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetCompletedCoursesRequest, List<Enrollment>>(serviceProvider)
{
    public override async Task<List<Enrollment>> Handle(GetCompletedCoursesRequest request, CancellationToken token)
    {
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student == null || student.IsMentor) return null;

        await using var context = GetDataContext();
        return await context.Enrollments
            .Where(e => e.StudentId == request.StudentId && e.IsCourseCompleted && e.ClosureDate.HasValue)
            .Include(e => e.CourseRevision)
            .ThenInclude(r => r.Course)
            .ToListAsync(cancellationToken: token);
    }
}