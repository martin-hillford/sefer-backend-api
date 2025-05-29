namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseStudentsCountHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseStudentsCountRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetCourseStudentsCountRequest request, CancellationToken token)
    {
        if (request.CourseId == null) return 0;
        await using var context = GetDataContext();

        return await context.Enrollments
            .CountAsync(e => e.CourseRevision.CourseId == request.CourseId, token);
    }
}