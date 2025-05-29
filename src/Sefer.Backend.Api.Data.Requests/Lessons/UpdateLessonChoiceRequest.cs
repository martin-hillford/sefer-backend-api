namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class UpdateLessonChoiceRequest(MultipleChoiceQuestionChoice entity)
    : UpdateEntityRequest<MultipleChoiceQuestionChoice>(entity);