namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddLessonHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddLessonRequest, Lesson>(serviceProvider);