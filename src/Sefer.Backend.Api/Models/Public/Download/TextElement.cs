// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class TextElement(LessonTextElement element) : ContentBlock(element)
{
    public string Content => element.Content;

    public bool IsMarkDownContent => element.IsMarkDownContent;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => element.Type;
}
