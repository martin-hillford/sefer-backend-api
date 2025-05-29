namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorsOfCourseRequest(int courseId) : IRequest<List<User>>
{
    public readonly int CourseId = courseId;
}