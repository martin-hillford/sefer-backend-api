namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonsForAudioReferenceIdHandler(IServiceProvider serviceProvider) : Handler<GetLessonsForAudioReferenceIdRequest, List<Lesson>>(serviceProvider)
{
    public override async Task<List<Lesson>> Handle(GetLessonsForAudioReferenceIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Lessons
            .Where(s => s.AudioReferenceId == request.AudioReferenceId)
            .ToListAsync(token);
    }
}




