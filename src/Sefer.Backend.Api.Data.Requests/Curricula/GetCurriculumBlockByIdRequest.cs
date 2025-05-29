namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumBlockByIdRequest(int? id) : GetEntityByIdRequest<CurriculumBlock>(id);