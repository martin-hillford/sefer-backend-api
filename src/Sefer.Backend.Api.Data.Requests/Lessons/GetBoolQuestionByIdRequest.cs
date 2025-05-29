namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetBoolQuestionByIdRequest(int? id) : GetEntityByIdRequest<BoolQuestion>(id);