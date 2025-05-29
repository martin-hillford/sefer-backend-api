namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetMultipleChoiceQuestionByIdRequest(int? id) : GetEntityByIdRequest<MultipleChoiceQuestion>(id);