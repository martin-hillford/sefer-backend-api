// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global
namespace Sefer.Backend.Api.Views.Public.Download;

public class MediaElement(Data.Models.Courses.Lessons.MediaElement element) : ContentBlock(element)
{
    public override string Content { get; set; } = element.Content;

    public bool IsMarkDownContent => element.IsMarkDownContent;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => element.Type;
    
    public string Url => element.Url;
}