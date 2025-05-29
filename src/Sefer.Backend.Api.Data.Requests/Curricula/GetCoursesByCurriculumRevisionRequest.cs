namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCoursesByCurriculumRevisionRequest(int curriculumRevisionId) : IRequest<List<Course>>
{
    public readonly int CurriculumRevisionId = curriculumRevisionId;
}