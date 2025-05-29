namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class IsCurriculumRevisionPublishableRequest(int curriculumRevisionId) : IRequest<bool>
{
    public readonly int CurriculumRevisionId = curriculumRevisionId;
}