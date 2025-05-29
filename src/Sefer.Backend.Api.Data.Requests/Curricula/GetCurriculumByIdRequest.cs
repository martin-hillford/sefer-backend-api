namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumByIdRequest(int? id, bool includeRevisions = false) : GetEntityByIdRequest<Curriculum>(id)
{
    public readonly bool IncludeRevisions = includeRevisions;
}