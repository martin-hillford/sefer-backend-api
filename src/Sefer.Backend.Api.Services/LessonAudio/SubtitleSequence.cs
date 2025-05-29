// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Services.LessonAudio;

public class SubtitleSequence
{
    public int Number;

    public string TimeCode { get; set; }

    public string Caption { get; set; }

    public string AudioFile { get; set; }

    public string AudioUrl { get; set; }
}