namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetPublishedCoursesOfMentorHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCoursesOfMentorRequest, List<CourseRevision>>(serviceProvider)
{
    public override async Task<List<CourseRevision>> Handle(GetPublishedCoursesOfMentorRequest request, CancellationToken token)
    {
        var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
        if (mentor?.IsMentor != true) return new List<CourseRevision>();

        var context = GetDataContext();
        var mentorCourse = context.MentorCourses.Where(c => c.MentorId == mentor.Id).Select(c => c.CourseId);
        return await context.CourseRevisions
            .AsNoTracking()
            .Where(c => mentorCourse.Contains(c.CourseId) && c.Stage == Stages.Published)
            .Include(c => c.Course)
            .OrderBy(c => c.Course.Name)
            .Select(c => c)
            .ToListAsync(token);
    }
}