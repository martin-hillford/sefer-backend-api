namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class ContentStateHandler(IServiceProvider serviceProvider)
    : Handler<ContentStateRequest, List<LessonContentState>>(serviceProvider)
{
    public override async Task<List<LessonContentState>> Handle(ContentStateRequest request, CancellationToken token)
    {
        var revision = await GetCourseRevisionAsync(request, token);
        if (revision == null) return null;

        var merged = await GetMerged(request, token);
        var lessons = await GetLessonsAsync(request, token);

        return lessons
            .Select(l => new LessonContentState { LessonId = l, ContentState = GetContentState(l, merged) })
            .ToList();
    }

    private async Task<List<int>> GetLessonsAsync(ContentStateRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Lessons
            .AsNoTracking()
            .Where(l => l.CourseRevisionId == request.CourseRevisionId)
            .Select(l => l.Id)
            .ToListAsync(token);
    }

    private async Task<CourseRevision> GetCourseRevisionAsync(ContentStateRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CourseRevisions.Where(r => r.Id == request.CourseRevisionId).SingleOrDefaultAsync(token);
    }

    private static ContentState GetContentState(int lessonId, IEnumerable<Result> results)
    {
        var lesson = results.Where(r => r.LessonId == lessonId).ToList();
        if (!lesson.Any()) return ContentState.Html;
        if (lesson.All(l => l.IsMarkDownContent)) return ContentState.MarkDown;
        return lesson.All(l => !l.IsMarkDownContent) ? ContentState.Html : ContentState.Mixed;
    }

    private async Task<List<Result>> GetMerged(ContentStateRequest request, CancellationToken cancellationToken)
    {
        await using var context = GetDataContext();

        var boolQuestions = await context.LessonBoolQuestions
            .AsNoTracking()
            .Where(q => q.Lesson.CourseRevisionId == request.CourseRevisionId)
            .Select(q => new Result(q.LessonId, q.IsMarkDownContent))
            .ToListAsync(cancellationToken);

        var mcQuestions = await context.LessonMultipleChoiceQuestions
            .AsNoTracking()
            .Where(q => q.Lesson.CourseRevisionId == request.CourseRevisionId)
            .Select(q => new Result(q.LessonId, q.IsMarkDownContent))
            .ToListAsync(cancellationToken);

        var openQuestions = await context.LessonOpenQuestions
            .AsNoTracking()
            .Where(q => q.Lesson.CourseRevisionId == request.CourseRevisionId)
            .Select(q => new Result(q.LessonId, q.IsMarkDownContent))
            .ToListAsync(cancellationToken);

        var mediaElements = await context.LessonMediaElements
            .AsNoTracking()
            .Where(q => q.Lesson.CourseRevisionId == request.CourseRevisionId)
            .Select(q => new Result(q.LessonId, q.IsMarkDownContent))
            .ToListAsync(cancellationToken);

        var textElements = await context.LessonTextElements
            .AsNoTracking()
            .Where(q => q.Lesson.CourseRevisionId == request.CourseRevisionId)
            .Select(q => new Result(q.LessonId, q.IsMarkDownContent))
            .ToListAsync(cancellationToken);

        return boolQuestions.Union(mcQuestions).Union(openQuestions).Union(mediaElements).Union(textElements).ToList();
    }

    private sealed class Result(int lessonId, bool isMarkDownContent)
    {
        public readonly int LessonId = lessonId;

        public readonly bool IsMarkDownContent = isMarkDownContent;
    }
}