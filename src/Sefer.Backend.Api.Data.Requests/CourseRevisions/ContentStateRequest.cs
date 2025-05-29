namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class ContentStateRequest(int courseRevisionId) : IRequest<List<LessonContentState>>
{
    public readonly int CourseRevisionId = courseRevisionId;
}