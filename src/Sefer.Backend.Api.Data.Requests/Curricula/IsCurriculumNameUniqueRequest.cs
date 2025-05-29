namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class IsCurriculumNameUniqueRequest(int? curriculumId, string name) : IRequest<bool>
{
    public readonly int? CurriculumId = curriculumId;

    public readonly string Name = name;
}