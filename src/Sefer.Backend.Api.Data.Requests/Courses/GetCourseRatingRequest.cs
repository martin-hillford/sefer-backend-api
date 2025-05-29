namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseRatingRequest(int? courseId) : IRequest<(byte rating, int count)>
{
    public readonly int? CourseId = courseId;
}