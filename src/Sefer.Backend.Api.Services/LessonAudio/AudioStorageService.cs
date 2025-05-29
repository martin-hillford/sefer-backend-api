namespace Sefer.Backend.Api.Services.LessonAudio;

public class AudioStorageService(IOptions<AudioStorageServiceOptions> options) : IAudioStorageService
{
    private readonly string _publicUrl = options?.Value.PublicUrl;

    private readonly string _blobSasUrl = options?.Value.BlobSasUrl;

    public async Task<bool> StoreAudioAsync(SubtitlesFile file, Guid audioReferenceId)
    {
        try
        {
            var subtitlesFile = $"{audioReferenceId}/captions.json";
            var container = new BlobContainerClient(new Uri(_blobSasUrl));

            // check if the subtitle file already exists
            var client = container.GetBlobClient(subtitlesFile);
            var exists = await client.ExistsAsync();
            if (exists) return false;

            // Now upload all the mp3 files in the captions file
            foreach (var sequence in file.Sequences)
            {
                var mp3File = Path.Combine(file.FileDirectory, sequence.AudioFile);
                var mp3Uri = $"{audioReferenceId}/{sequence.AudioFile}";
                var mp3Blob = container.GetBlobClient(mp3Uri);
                await mp3Blob.UploadAsync(mp3File);
                sequence.AudioUrl = $"{_publicUrl}/{mp3Uri}"; // write the url to the sequence so it gets write to the json later
            }

            // Write the json of the captions file. Ensure to include the audioReferenceId
            file.AudioReferenceId = audioReferenceId;
            var json = JsonSerializer.Serialize(file, JsonOptions).ToStream();
            await client.UploadAsync(json);

            return true;
        }
        catch (Exception) { return false; }
    }

    public async Task<SubtitlesFile> RetrieveAudioAsync(Guid audioReferenceId)
    {
        try
        {
            var (file, _, _) = await GetAudioAsync(audioReferenceId);
            return file;
        }
        catch (Exception) { return null; }
    }

    public async Task<bool> DeleteAudioAsync(Guid audioReferenceId)
    {
        try
        {
            var (file, container, client) = await GetAudioAsync(audioReferenceId);

            // Delete mp3 files in the captions file
            foreach (var sequence in file.Sequences)
            {
                var mp3Uri = $"{audioReferenceId}/{sequence.AudioFile}";
                var mp3Blob = container.GetBlobClient(mp3Uri);
                var deleteResult = await mp3Blob.DeleteIfExistsAsync();
                if (!deleteResult.Value) return false;
            }

            var result = await client.DeleteIfExistsAsync();
            return result.Value;

        }
        catch (Exception) { return false; }
    }

    private async Task<(SubtitlesFile, BlobContainerClient, BlobClient)> GetAudioAsync(Guid audioReferenceId)
    {
        var container = new BlobContainerClient(new Uri(_blobSasUrl));
        var subtitlesFile = $"{audioReferenceId}/captions.json";
        var client = container.GetBlobClient(subtitlesFile);
        var contents = await client.DownloadStringAsync();
        var file = JsonSerializer.Deserialize<SubtitlesFile>(contents, JsonOptions);
        return (file, container, client);
    }

    private static readonly JsonSerializerOptions JsonOptions = DefaultJsonOptions.GetOptions();
}