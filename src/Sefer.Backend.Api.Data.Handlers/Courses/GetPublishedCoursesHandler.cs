namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetPublishedCoursesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.CourseRevisions
            .Include(r => r.Course)
            .Where(r => r.Stage == Stages.Published)
            .OrderBy(r => r.Course.Name)
            .Select(r => r.Course)
            .ToListAsync(token);
    }
}