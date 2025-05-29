namespace Sefer.Backend.Api.Services.LessonAudio;

public interface IAudioStorageService
{
    public Task<bool> StoreAudioAsync(SubtitlesFile file, Guid audioReferenceId);

    public Task<SubtitlesFile> RetrieveAudioAsync(Guid audioReferenceId);

    public Task<bool> DeleteAudioAsync(Guid audioReferenceId);
}