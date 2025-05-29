namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCoursesForLevelHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesForLevelRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetPublishedCoursesForLevelRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.CourseRevisions
            .Include(r => r.Course)
            .Where(r => r.Stage == Stages.Published && r.Course.Level == request.Level)
            .OrderBy(r => r.Course.Name)
            .Select(r => r.Course)
            .ToListAsync(token);
    }
}