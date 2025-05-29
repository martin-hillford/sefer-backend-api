namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class SyncLessonAudioToPredecessorRequest(int lessonId) : IRequest<bool>
{
    public readonly int LessonId = lessonId;
}