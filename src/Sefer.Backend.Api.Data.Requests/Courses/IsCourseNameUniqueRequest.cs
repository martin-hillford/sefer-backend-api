namespace Sefer.Backend.Api.Data.Requests.Courses;

public class IsCourseNameUniqueRequest(int? courseId, string name) : IRequest<bool>
{
    public readonly string Name = name;

    public readonly int? CourseId = courseId;
}