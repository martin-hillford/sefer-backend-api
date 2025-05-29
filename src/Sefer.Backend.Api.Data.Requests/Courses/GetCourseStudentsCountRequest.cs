namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseStudentsCountRequest(int? courseId) : IRequest<int>
{
    public readonly int? CourseId = courseId;
}