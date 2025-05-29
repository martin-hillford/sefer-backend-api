namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetStudentsCountPerCourseHandler(IServiceProvider serviceProvider)
    : Handler<GetStudentsCountPerCourseRequest, Dictionary<int, int>>(serviceProvider)
{
    public override async Task<Dictionary<int, int>> Handle(GetStudentsCountPerCourseRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.CourseStudentCount.ToDictionaryAsync(c => c.CourseId, c => c.Count, token);
    }
}