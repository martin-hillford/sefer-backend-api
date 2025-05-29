namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetHomepageCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetHomepageCoursesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetHomepageCoursesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions
            .AsNoTracking()
            .Include(r => r.Course)
            .Where(r => r.Stage == Stages.Published && r.Course.ShowOnHomepage)
            .OrderByDescending(r => r.Course.CreationDate)
            .Select(r => r.Course)
            .ToListAsync(token);
    }
}