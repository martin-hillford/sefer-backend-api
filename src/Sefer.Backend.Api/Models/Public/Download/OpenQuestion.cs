// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class OpenQuestion(Data.Models.Courses.Lessons.OpenQuestion openQuestion) : ContentBlock(openQuestion)
{
    public string Content => openQuestion.Content;

    public bool IsMarkDownContent => openQuestion.IsMarkDownContent;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => openQuestion.Type;
}