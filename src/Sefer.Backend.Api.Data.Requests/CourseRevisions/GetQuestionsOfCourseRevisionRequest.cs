namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetQuestionsOfCourseRevisionRequest(int courseRevisionId) : IRequest<List<(Lesson Lesson, List<Question> Questions)>>
{
    public readonly int CourseRevisionId = courseRevisionId;
}