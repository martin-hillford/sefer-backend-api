namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetOpenQuestionByIdRequest(int? id) : GetEntityByIdRequest<OpenQuestion>(id);