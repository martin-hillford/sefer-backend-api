namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetMentorCoursesRequest(int mentorId) : IRequest<List<Course>>
{
    public readonly int MentorId = mentorId;
}