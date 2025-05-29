namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class UpdateMultipleChoiceQuestionRequest(MultipleChoiceQuestion entity)
    : UpdateEntityRequest<MultipleChoiceQuestion>(entity);