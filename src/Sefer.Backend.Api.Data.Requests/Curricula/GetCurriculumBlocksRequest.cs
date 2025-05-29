namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumBlocksRequest(int curriculumRevisionId) : IRequest<List<CurriculumBlock>>
{
    public readonly int CurriculumRevisionId = curriculumRevisionId;
}