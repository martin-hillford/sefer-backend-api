namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumOverallStageRequest(int curriculumId) : IRequest<Stages?>
{
    public readonly int CurriculumId = curriculumId;
}