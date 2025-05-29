namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetRequiredCoursesRequest(int courseId) : IRequest<List<Course>>
{
    public readonly int CourseId = courseId;
}