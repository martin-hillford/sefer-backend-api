namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class CloseCourseRevisionRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}