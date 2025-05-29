namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCourseByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCourseByIdRequest, Course>(serviceProvider)
{
    public override async Task<Course> Handle(GetPublishedCourseByIdRequest request, CancellationToken token)
    {
        if (request.CourseId == null) return null;
        var context = GetDataContext();
        return await context.CourseRevisions
            .Where(r => r.Stage == Stages.Published && r.CourseId == request.CourseId)
            .Include(r => r.Course)
            .ThenInclude(c => c.CourseRevisions)
            .Select(r => r.Course)
            .SingleOrDefaultAsync(token);
    }
}