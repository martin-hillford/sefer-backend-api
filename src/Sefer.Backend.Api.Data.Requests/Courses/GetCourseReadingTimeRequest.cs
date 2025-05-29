namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseReadingTimeRequest(int? courseId = null) : IRequest<Dictionary<int, CourseReadingTime>>
{
    public readonly int? CourseId = courseId;
}