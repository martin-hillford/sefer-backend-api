namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetClosedCurriculumRevisionsRequest(int curriculumId) : IRequest<List<CurriculumRevision>>
{
    public readonly int CurriculumId = curriculumId;
}