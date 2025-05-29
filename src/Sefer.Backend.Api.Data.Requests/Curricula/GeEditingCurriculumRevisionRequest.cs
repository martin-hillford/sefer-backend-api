namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetEditingCurriculumRevisionRequest(int curriculumId) : IRequest<CurriculumRevision>
{
    public readonly int CurriculumId = curriculumId;
}