namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class IsChoiceValidRequest(MultipleChoiceQuestionChoice entity)
    : IsValidEntityRequest<MultipleChoiceQuestionChoice>(entity);