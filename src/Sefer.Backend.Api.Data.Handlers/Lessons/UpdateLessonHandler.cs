namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateLessonHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateLessonRequest, Lesson>(serviceProvider);