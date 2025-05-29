namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class IsLessonValidHandler(IServiceProvider serviceProvider)
    : IsValidEntityHandler<IsLessonValidRequest, Lesson>(serviceProvider);