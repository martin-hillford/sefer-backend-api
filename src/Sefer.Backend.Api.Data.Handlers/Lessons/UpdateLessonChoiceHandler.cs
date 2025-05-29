namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateLessonChoiceHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateLessonChoiceRequest, MultipleChoiceQuestionChoice>(serviceProvider);