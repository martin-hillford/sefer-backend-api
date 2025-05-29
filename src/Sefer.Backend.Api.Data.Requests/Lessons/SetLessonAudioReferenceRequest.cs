namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class SetLessonAudioReferenceRequest(int lessonId, Guid? audioReferenceId) : IRequest<bool>
{
    public readonly int LessonId = lessonId;

    public readonly Guid? AudioReferenceId = audioReferenceId;
}