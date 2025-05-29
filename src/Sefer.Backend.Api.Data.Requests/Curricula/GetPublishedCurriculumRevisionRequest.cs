namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetPublishedCurriculumRevisionRequest(int curriculumId) : IRequest<CurriculumRevision>
{
    public readonly int CurriculumId = curriculumId;
}