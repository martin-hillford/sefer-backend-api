namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetEditingCourseRevisionRequest(int courseId) : IRequest<CourseRevision>
{
    public readonly int CourseId = courseId;
}