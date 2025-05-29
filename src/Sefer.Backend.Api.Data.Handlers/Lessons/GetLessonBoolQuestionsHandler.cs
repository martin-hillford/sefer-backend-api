namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonBoolQuestionsHandler(IServiceProvider serviceProvider)
    : GetLessonContentBlocksHandler<GetLessonBoolQuestionsRequest, BoolQuestion>(serviceProvider);