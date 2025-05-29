namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetPublishedCoursesForLevelRequest(Levels level) : IRequest<List<Course>>
{
    public readonly Levels Level = level;
}