namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class SyncLessonAudioToPredecessorHandler(IServiceProvider serviceProvider) : Handler<SyncLessonAudioToPredecessorRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(SyncLessonAudioToPredecessorRequest request, CancellationToken token)
    {
        try
        {
            SyncLessonAudioToPredecessor(request.LessonId);
            return Task.FromResult(true);
        }
        catch (Exception) { return Task.FromResult(false); }
    }

    private void SyncLessonAudioToPredecessor(int lessonId)
    {
        var context = GetDataContext();
        var lesson = context.Lessons.SingleOrDefault(s => s.Id == lessonId);
        if (lesson == null || !lesson.AudioReferenceId.HasValue || !lesson.PredecessorId.HasValue) return;

        var predecessor = context.Lessons.SingleOrDefault(s => s.Id == lesson.PredecessorId);
        if (predecessor == null) return;

        predecessor.AudioReferenceId = lesson.AudioReferenceId;
        context.UpdateSingleProperty(predecessor, "AudioReferenceId");
        context.Dispose();

        if (predecessor.PredecessorId.HasValue) SyncLessonAudioToPredecessor(predecessor.Id);
    }
}