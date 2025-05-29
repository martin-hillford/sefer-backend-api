namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddTextElementHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddTextElementRequest, LessonTextElement>(serviceProvider);