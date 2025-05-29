namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddLessonChoiceHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddLessonChoiceRequest, MultipleChoiceQuestionChoice>(serviceProvider);