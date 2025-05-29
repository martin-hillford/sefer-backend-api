namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public sealed class DeleteLessonHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteLessonRequest, Lesson>(serviceProvider);