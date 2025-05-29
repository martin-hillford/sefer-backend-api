namespace Sefer.Backend.Api.Data.Handlers.Students;

public class GetPersonalMentorForCourseHandler(IServiceProvider serviceProvider)
    : Handler<GetPersonalMentorForCourseRequest, User>(serviceProvider)
{
    public override async Task<User> Handle(GetPersonalMentorForCourseRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.StudentSettings
            .Where(s => s.StudentId == request.StudentId)
            .Where(s => s.PersonalMentor != null)
            .Where(m => m.PersonalMentor.MentorCourses.Any(a => a.CourseId == request.CourseId))
            .Select(s => s.PersonalMentor)
            .FirstOrDefaultAsync(token);
    }
}