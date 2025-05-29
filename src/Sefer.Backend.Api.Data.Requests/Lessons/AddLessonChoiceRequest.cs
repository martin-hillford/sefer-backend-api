namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class AddLessonChoiceRequest(MultipleChoiceQuestionChoice entity)
    : AddEntityRequest<MultipleChoiceQuestionChoice>(entity);