// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
using System.IO.Compression;
using System.Text.Json.Serialization;

namespace Sefer.Backend.Api.Services.LessonAudio;

public class SubtitlesFile
{
    public List<SubtitleSequence> Sequences { get; set; }

    [JsonIgnore]
    public string FileDirectory { get; set; }

    public Guid AudioReferenceId { get; set; }

    public static async Task<(bool, string)> CopyAndExtractAsync(IFormFile audio, Guid audioReferenceId)
    {
        try
        {
            var tempFile = Path.GetTempFileName();
            var outputDirectory = FileUtils.GetTemporaryDirectory(audioReferenceId.ToString());
            await using var fileStream = new FileStream(tempFile, FileMode.Open);
            await audio.CopyToAsync(fileStream);
            ZipFile.ExtractToDirectory(tempFile, outputDirectory);
            return (true, outputDirectory);
        }
        catch (Exception)
        {
            return (false, null);
        }
    }

    public static async Task<SubtitlesFile> ReadAudioDirectoryAsync(string outputDirectory)
    {
        try
        {
            // First read and parse the srt file
            var files = Directory.GetFiles(outputDirectory);
            var srtFile = files.Single(f => f.EndsWith(".srt"));
            var subtitlesFile = await GetContentsFromFileAsync(srtFile);

            // Now check if from every caption sequence a mp3 file can be found
            var mp3Files = files.Where(f => f.EndsWith(".mp3")).Select(f => new FileInfo(f)).ToList();
            foreach (var sequence in subtitlesFile.Sequences)
            {
                var fileInfo = mp3Files.Find(f => f.Name.StartsWith(sequence.Number + "."));
                sequence.AudioFile = fileInfo.Name;
            }

            subtitlesFile.FileDirectory = outputDirectory;
            return subtitlesFile;
        }
        catch (Exception) { return null; }
    }

    private static async Task<SubtitlesFile> GetContentsFromFileAsync(string fileName)
    {
        var contents = await File.ReadAllTextAsync(fileName);
        return GetContents(contents);
    }

    private static SubtitlesFile GetContents(string contents)
    {
        // Do a cleanup of the new lines
        contents = contents.Replace("\n\r", "\n").Replace("\r\n", "\n").Trim();

        // Blank lines will act as separator between caption sequences
        var sequences = contents.Split("\n\n").Select(ParseSequence).ToList();

        // Validate that the sequence ids are increasing with + 1
        var increasing = sequences.Select((s, index) => s.Number == index + 1).All(s => s);
        return !increasing ? null : new SubtitlesFile { Sequences = sequences };
    }

    private static SubtitleSequence ParseSequence(string sequence)
    {
        // split by line
        var lines = sequence.Split("\n");

        return new SubtitleSequence
        {
            Number = int.Parse(lines[0].Trim()),
            TimeCode = lines[1].Trim(),
            Caption = string.Join("\n", lines[2..]).Trim()
        };
    }
}