namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetLessonsByCourseRevisionRequest(int courseRevisionId) : IRequest<List<Lesson>>
{
    public readonly int CourseRevisionId = courseRevisionId;
}