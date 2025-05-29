namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class SetLessonAudioReferenceHandler(IServiceProvider serviceProvider) : Handler<SetLessonAudioReferenceRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(SetLessonAudioReferenceRequest request, CancellationToken token)
    {
        try
        {
            var context = GetDataContext();
            var lesson = await context.Lessons.SingleAsync(s => s.Id == request.LessonId, token);
            lesson.AudioReferenceId = request.AudioReferenceId;
            context.UpdateSingleProperty(lesson, "AudioReferenceId");
            return true;
        }
        catch (Exception) { return false; }
    }
}