namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class SetMentorForCourseRequest(int courseId, int mentorId) : IRequest<bool>
{
    public readonly int CourseId = courseId;

    public readonly int MentorId = mentorId;
}