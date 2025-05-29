namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class IsPublishableCourseRevisionRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}