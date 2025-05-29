namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonIncludeReferencesRequest(int? lessonId) : IRequest<Lesson>
{
    public readonly int? LessonId = lessonId;
}