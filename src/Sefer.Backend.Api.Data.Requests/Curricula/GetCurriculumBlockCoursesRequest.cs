namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumBlockCoursesRequest(int curriculumBlockId) : IRequest<List<CurriculumBlockCourse>>
{
    public readonly int CurriculumBlockId = curriculumBlockId;
}