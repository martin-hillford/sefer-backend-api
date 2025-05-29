namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetOverallCourseStageRequest(int courseId) : IRequest<Stages?>
{
    public readonly int CourseId = courseId;
}