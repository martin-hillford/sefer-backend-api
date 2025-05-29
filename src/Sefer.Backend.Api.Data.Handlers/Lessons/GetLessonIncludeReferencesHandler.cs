namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonIncludeReferencesHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonIncludeReferencesRequest, Lesson>(serviceProvider)
{
    public override async Task<Lesson> Handle(GetLessonIncludeReferencesRequest request, CancellationToken token)
    {
        if (request.LessonId == null) return null;

        await using var context = GetDataContext();
        var lessons = context.Lessons.Where(l => l.Id == request.LessonId);
        if (!await lessons.AnyAsync(token)) return null;

        await lessons.Include(l => l.CourseRevision).ThenInclude(c => c.Course).LoadAsync(token);
        var lesson = lessons.First();

        lesson.Content = await Send(new GetLessonContentRequest(lesson), token);
        return lesson;
    }
}