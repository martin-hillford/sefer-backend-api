// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class MediaElement(Data.Models.Courses.Lessons.MediaElement element) : ContentBlock(element)
{
    public string Content => element.Content;

    public bool IsMarkDownContent => element.IsMarkDownContent;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => element.Type;
    
    public string Url => element.Url;
}