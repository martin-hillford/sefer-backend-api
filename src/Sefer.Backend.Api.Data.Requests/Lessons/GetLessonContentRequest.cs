namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonContentRequest(Lesson lesson) : IRequest<List<IContentBlock<Lesson>>>
{
    public readonly Lesson Lesson = lesson;
}