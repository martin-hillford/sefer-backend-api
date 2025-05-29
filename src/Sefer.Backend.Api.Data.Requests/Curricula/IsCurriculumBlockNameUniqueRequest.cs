namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class IsCurriculumBlockNameUniqueRequest(int? id, int curriculumId, int? year, string name)
    : IRequest<bool>
{
    public readonly int? BlockId = id;

    public readonly int CurriculumId = curriculumId;

    public readonly int? Year = year;

    public readonly string Name = name;
}