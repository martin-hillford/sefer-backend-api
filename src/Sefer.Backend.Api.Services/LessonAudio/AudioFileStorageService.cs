namespace Sefer.Backend.Api.Services.LessonAudio;

public class AudioFileStorageService(IOptions<AudioStorageServiceOptions> options) : IAudioStorageService
{
    private readonly string _audioRoot = options?.Value.FileStoragePath;

    private readonly string _publicUrl = options?.Value.PublicUrl;
    
    private static readonly JsonSerializerOptions JsonOptions = DefaultJsonOptions.GetOptions();

    public async Task<bool> StoreAudioAsync(SubtitlesFile file, Guid audioReferenceId)
    {
        try
        {
            // The directory where the files are stored
            var directory = audioReferenceId.ToString();

            // check if the subtitle file already exists
            var subtitlesFile = Path.Combine(_audioRoot, directory, "captions.json");
            if (!File.Exists(subtitlesFile)) return false;

            // Now upload all the mp3 files in the caption file
            foreach (var sequence in file.Sequences)
            {
                var sourcePath = Path.Combine(file.FileDirectory, sequence.AudioFile);
                var mp3Uri = $"{audioReferenceId}/{sequence.AudioFile}";

                var targetPath = Path.Combine(_audioRoot, directory, sequence.AudioFile);
                File.Copy(sourcePath, targetPath);
                sequence.AudioUrl =
                    $"{_publicUrl}/{mp3Uri}"; // write the url to the sequence so it gets write to the JSON later
            }

            // Write the JSON of the caption file. Ensure to include the audioReferenceId
            file.AudioReferenceId = audioReferenceId;
            var json = JsonSerializer.Serialize(file, JsonOptions);
            await File.WriteAllTextAsync(subtitlesFile, json);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<SubtitlesFile> RetrieveAudioAsync(Guid audioReferenceId)
    {
        try
        {
            var subtitlesFile = Path.Combine(_audioRoot, audioReferenceId.ToString(), "captions.json");
            var contents = await File.ReadAllTextAsync(subtitlesFile);
            return JsonSerializer.Deserialize<SubtitlesFile>(contents, JsonOptions);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public Task<bool> DeleteAudioAsync(Guid audioReferenceId)
    {
        try
        {
            // The directory where the files are stored
            var directory = Path.Combine(_audioRoot, audioReferenceId.ToString());
            if (!Directory.Exists(directory)) return Task.FromResult(false);

            Directory.Delete(directory, true);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }
    
    public ActionResult GetAudioFile(Guid audioReferenceId, string fileName)
    {
        var filePath = Path.Combine(_audioRoot, audioReferenceId.ToString(), fileName);
        if (!File.Exists(filePath)) return new NotFoundResult();

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var contentType = GetMimeType(Path.GetExtension(filePath));

        return new FileStreamResult(fileStream, contentType);
    }

    private static string GetMimeType(string fileExtension)
    {
        return fileExtension.ToLower() switch
        {
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".aac" => "audio/aac",
            ".ogg" => "audio/ogg",
            ".flac" => "audio/flac",
            ".m4a" => "audio/mp4",
            ".wma" => "audio/x-ms-wma",
            ".aiff" => "audio/aiff",
            ".amr" => "audio/amr",
            ".opus" => "audio/opus",
            _ => "application/octet-stream"
        };
    }
}