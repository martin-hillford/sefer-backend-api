namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseByIdExtendedRequest(int courseId) : IRequest<Course>
{
    public readonly int CourseId = courseId;
}