namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCoursesWithRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetCoursesWithRevisionRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetCoursesWithRevisionRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Courses
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Include(c => c.CourseRevisions)
            .ToListAsync(token);
    }
}