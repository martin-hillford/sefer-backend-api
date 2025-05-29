namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonsForAudioReferenceIdRequest(Guid audioReferenceId) : IRequest<List<Lesson>>
{
    public readonly Guid AudioReferenceId = audioReferenceId;
}