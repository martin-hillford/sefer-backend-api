namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetPublishedCoursesOfMentorRequest(int mentorId) : IRequest<List<CourseRevision>>
{
    public readonly int MentorId = mentorId;
}