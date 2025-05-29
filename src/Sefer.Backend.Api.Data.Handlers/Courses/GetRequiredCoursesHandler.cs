namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetRequiredCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetRequiredCoursesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetRequiredCoursesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.CoursePrerequisites
            .Where(p => p.CourseId == request.CourseId)
            .OrderBy(p => p.RequiredCourse.Name)
            .Select(p => p.RequiredCourse)
            .ToListAsync(token);
    }
}