namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCoursesByCurriculumBlockRequest(int curriculumBlockId, bool publishedOnly) : IRequest<List<Course>>
{
    public readonly int CurriculumBlockId = curriculumBlockId;

    public readonly bool PublishedOnly = publishedOnly;
}