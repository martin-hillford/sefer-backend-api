namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseDictionaryRequest(int courseRevisionId) : IRequest<List<CourseRevisionDictionaryWord>>
{
    public readonly int CourseRevisionId = courseRevisionId;
}