namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetPublishedCourseByIdRequest(int? courseId) : IRequest<Course>
{
    public readonly int? CourseId = courseId;
}