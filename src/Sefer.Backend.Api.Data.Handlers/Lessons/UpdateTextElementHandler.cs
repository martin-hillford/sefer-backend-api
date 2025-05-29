namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateTextElementHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateTextElementRequest, LessonTextElement>(serviceProvider);