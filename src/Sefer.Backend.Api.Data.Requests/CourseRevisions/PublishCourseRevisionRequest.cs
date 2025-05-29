namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class PublishCourseRevisionRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}