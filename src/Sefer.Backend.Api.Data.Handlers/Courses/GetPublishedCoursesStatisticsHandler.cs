namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetPublishedCoursesStatisticsHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesStatisticsRequest, List<CourseStat>>(serviceProvider)
{
    public override async Task<List<CourseStat>> Handle(GetPublishedCoursesStatisticsRequest request, CancellationToken token)
    {
        // First get the courses
        var revisions = await GetCourses(token);
        var lessonCount = await GetCourseLessonCount(token);
        var studentCount = await GetStudentCount(token);

        return revisions.Select(r =>
                new CourseStat
                {
                    Course = r.Course,
                    Lessons = lessonCount[r.CourseId],
                    Students = studentCount[r.CourseId],
                })
            .ToList();
    }

    private async Task<List<CourseRevision>> GetCourses(CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions
            .Where(r => r.Stage == Stages.Published)
            .Include(r => r.Course)
            .ToListAsync(token);
    }

    private async Task<Dictionary<int, int>> GetCourseLessonCount(CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions
            .Where(r => r.Stage == Stages.Published)
            .Select(r => new { r.CourseId, LessonCount = r.Lessons.Count })
            .ToDictionaryAsync(r => r.CourseId, r => r.LessonCount, token);
    }
    
    private async Task<Dictionary<int, int>> GetStudentCount(CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseStudentCount.ToDictionaryAsync(c => c.CourseId, c => c.Count, token);
    }
}