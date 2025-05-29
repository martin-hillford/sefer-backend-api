namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetLessonsCountForCourseRevisionsHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonsCountForCourseRevisionsRequest, Dictionary<int, int>>(serviceProvider)
{
    public override async Task<Dictionary<int, int>> Handle(GetLessonsCountForCourseRevisionsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Lessons
            .GroupBy(l => l.CourseRevisionId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), token);
    }
}