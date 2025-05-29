namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonMediaElementsHandler(IServiceProvider serviceProvider)
    : GetLessonContentBlocksHandler<GetLessonMediaElementsRequest, MediaElement>(serviceProvider);