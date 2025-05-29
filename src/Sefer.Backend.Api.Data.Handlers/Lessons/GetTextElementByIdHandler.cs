namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetTextElementByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetTextElementByIdRequest, LessonTextElement>(serviceProvider);