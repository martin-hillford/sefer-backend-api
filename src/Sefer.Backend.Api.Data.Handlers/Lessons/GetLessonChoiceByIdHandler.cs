namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonChoiceByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetLessonChoiceByIdRequest, MultipleChoiceQuestionChoice>(serviceProvider);