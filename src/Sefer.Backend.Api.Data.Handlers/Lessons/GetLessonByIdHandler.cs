namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetLessonByIdRequest, Lesson>(serviceProvider);