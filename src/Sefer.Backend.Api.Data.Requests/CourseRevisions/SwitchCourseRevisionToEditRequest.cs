namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class SwitchCourseRevisionToEditRequest(int courseRevisionId) : IRequest<bool>
{
    public readonly int CourseRevisionId = courseRevisionId;
}