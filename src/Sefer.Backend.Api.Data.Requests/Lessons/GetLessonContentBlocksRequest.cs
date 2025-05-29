namespace Sefer.Backend.Api.Data.Requests.Lessons;

public abstract class GetLessonContentBlocksRequest<TContentBlock>(Lesson lesson) : IRequest<List<TContentBlock>>
    where TContentBlock : class, IContentBlock<Lesson, TContentBlock>
{
    public readonly int LessonId = lesson.Id;
}
