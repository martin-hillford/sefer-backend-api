namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class AddMultipleChoiceQuestionRequest(MultipleChoiceQuestion entity)
    : AddEntityRequest<MultipleChoiceQuestion>(entity);