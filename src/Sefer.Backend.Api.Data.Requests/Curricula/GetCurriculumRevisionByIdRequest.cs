namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumRevisionByIdRequest(int? id) : GetEntityByIdRequest<CurriculumRevision>(id);