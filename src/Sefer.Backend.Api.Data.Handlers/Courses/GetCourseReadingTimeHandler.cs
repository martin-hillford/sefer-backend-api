namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseReadingTimeHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseReadingTimeRequest, Dictionary<int, CourseReadingTime>>(serviceProvider)
{
    public override async Task<Dictionary<int, CourseReadingTime>> Handle(GetCourseReadingTimeRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var query = context.Set<CourseReadingTime>().AsQueryable();
        
        if (request.CourseId != null) query = query.Where(c => c.CourseId == request.CourseId);
        return await query.ToDictionaryAsync(c => c.CourseId, token);
    }
}