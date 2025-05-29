namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetLessonsCountOfCourseRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonsCountOfCourseRevisionRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetLessonsCountOfCourseRevisionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Lessons.CountAsync(l => l.CourseRevisionId == request.CourseRevisionId, token);
    }
}