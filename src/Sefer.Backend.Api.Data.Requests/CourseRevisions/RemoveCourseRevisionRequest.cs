namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class RemoveCourseRevisionRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}