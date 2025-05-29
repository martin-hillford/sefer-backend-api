namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetLastClosedEnrollmentHandler(IServiceProvider serviceProvider)
    : Handler<GetLastClosedEnrollmentRequest, Enrollment>(serviceProvider)
{
    public override async Task<Enrollment> Handle(GetLastClosedEnrollmentRequest request, CancellationToken token)
    {
        var student = await Send(new GetUserByIdRequest(request.StudentId), token);
        if (student == null || student.IsMentor) return null;

        await using var context = GetDataContext();
        return await context.Enrollments
            .Where(e => e.ClosureDate.HasValue && e.StudentId == student.Id)
            .Include(e => e.CourseRevision)
            .ThenInclude(c => c.Course)
            .OrderByDescending(e => e.ClosureDate)
            .FirstOrDefaultAsync(token);
    }
}