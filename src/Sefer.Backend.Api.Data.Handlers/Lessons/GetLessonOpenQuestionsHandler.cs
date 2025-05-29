namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonOpenQuestionsHandler(IServiceProvider serviceProvider)
    : GetLessonContentBlocksHandler<GetLessonOpenQuestionsRequest, OpenQuestion>(serviceProvider);