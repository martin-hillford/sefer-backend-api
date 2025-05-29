namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class IsCourseInPublishedCurriculumRequest(int courseId) : IRequest<bool>
{
    public readonly int CourseId = courseId;
}