namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculaRequest(bool includeRevisions = false) : IRequest<List<Curriculum>>
{
    public readonly bool IncludeRevisions = includeRevisions;
}