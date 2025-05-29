namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetLessonsCountOfCourseRevisionRequest(int courseRevisionId) : IRequest<int>
{
    public readonly int CourseRevisionId = courseRevisionId;
}