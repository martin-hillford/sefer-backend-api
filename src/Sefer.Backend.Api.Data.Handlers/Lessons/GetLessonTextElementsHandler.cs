namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonTextElementsHandler(IServiceProvider serviceProvider)
    : GetLessonContentBlocksHandler<GetLessonTextElementsRequest, LessonTextElement>(serviceProvider);