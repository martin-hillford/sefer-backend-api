namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorsOfCourseHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorsOfCourseRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetMentorsOfCourseRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorCourses
            .AsNoTracking()
            .Where(m => m.CourseId == request.CourseId)
            .OrderBy(m => m.Course.Name)
            .Include(m => m.Mentor)
            .ThenInclude(m => m.MentorSettings)
            .Select(m => m.Mentor)
            .ToListAsync(token);
    }
}