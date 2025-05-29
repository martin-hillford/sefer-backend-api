namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumBlocksByYearRequest(int curriculumRevisionId, short? year) : IRequest<List<CurriculumBlock>>
{
    public readonly int CurriculumRevisionId = curriculumRevisionId;

    public readonly short? Year = year;
}