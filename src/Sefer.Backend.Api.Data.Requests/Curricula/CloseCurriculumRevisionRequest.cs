namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class CloseCurriculumRevisionRequest(int curriculumRevisionId) : IRequest<bool>
{
    public readonly int CurriculumRevisionId = curriculumRevisionId;
}