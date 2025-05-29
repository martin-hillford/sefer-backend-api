namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetClosedCourseRevisionsRequest(int courseId) : IRequest<List<CourseRevision>>
{
    public readonly int CourseId = courseId;
}