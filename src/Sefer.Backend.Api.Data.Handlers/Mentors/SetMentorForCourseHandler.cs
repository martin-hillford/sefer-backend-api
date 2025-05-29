namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class SetMentorForCourseHandler(IServiceProvider serviceProvider)
    : Handler<SetMentorForCourseRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SetMentorForCourseRequest request, CancellationToken token)
    {
        try
        {
            // Check if we have a mentor and a course
            var course = await Send(new GetCourseByIdRequest(request.CourseId), token);
            var mentor = await Send(new GetUserByIdRequest(request.MentorId), token);
            if (mentor == null || mentor.IsMentor == false || course == null || mentor.Id < 1 || course.Id < 1) return false;

            var context = GetDataContext();

            // Check if the course is already mentored by the mentor, then true should be returned
            var exists = context.MentorCourses
                .Any(m => m.CourseId == course.Id && m.MentorId == mentor.Id);
            if (exists) return true;

            // Create the MentorCourse and add it
            var mentorCourse = new MentorCourse { CourseId = course.Id, MentorId = mentor.Id };
            context.MentorCourses.Add(mentorCourse);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception) { return false; }
    }
}