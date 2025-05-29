namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetCourseMentorsHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseMentorsRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetCourseMentorsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.MentorCourses
            .AsNoTracking()
            .Where(mc => mc.CourseId == request.CourseId)
            .Include(mc => mc.Mentor).ThenInclude(m => m.MentorSettings)
            .OrderBy(mc => mc.Mentor.Name)
            .Select(mc => mc.Mentor)
            .ToListAsync(token);
    }
}