namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetPublishedCourseRevisionRequest(int courseId) : IRequest<CourseRevision>
{
    public readonly int CourseId = courseId;
}