namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class RemoveMentorForCourseHandler(IServiceProvider serviceProvider)
    : Handler<RemoveMentorForCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(RemoveMentorForCourseRequest request, CancellationToken token)
    {
        try
        {
            // Check if we have a mentor and a course
            var course = await Send(new GetCourseByIdRequest(request.CourseId), token);
            var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
            if (mentor == null || mentor.IsMentor == false || course == null || mentor.Id < 1 || course.Id < 1) return false;

            // Check if the course is already mentored by the mentor, then true should be returned
            var context = GetDataContext();
            var exists = context.MentorCourses
                .Where(m => m.CourseId == course.Id && m.MentorId == mentor.Id)
                .ToList();
            if (exists.Count == 0) return true;

            // Create the MentorCourse and add it
            context.MentorCourses.RemoveRange(exists);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}