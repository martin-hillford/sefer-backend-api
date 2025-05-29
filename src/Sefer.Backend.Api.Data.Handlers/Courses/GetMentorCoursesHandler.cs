namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetMentorCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorCoursesRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetMentorCoursesRequest request, CancellationToken token)
    {
        var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (mentor == null || mentor.IsMentor == false) { return new List<Course>(); }

        await using var context = GetDataContext();
        return await context.MentorCourses
            .AsNoTracking()
            .Where(m => m.MentorId == request.MentorId)
            .OrderBy(m => m.Course.Name)
            .Select(m => m.Course)
            .ToListAsync(token);
    }
}