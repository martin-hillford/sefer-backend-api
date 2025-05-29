namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class SwitchCourseRevisionToTestRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}