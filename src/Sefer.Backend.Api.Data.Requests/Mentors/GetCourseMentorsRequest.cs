namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetCourseMentorsRequest(int courseId) : IRequest<List<User>>
{
    public readonly int CourseId = courseId;
}