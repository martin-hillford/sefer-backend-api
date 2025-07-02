// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class MultipleChoiceQuestion(Data.Models.Courses.Lessons.MultipleChoiceQuestion question) : ContentBlock(question)
{
    public string Content => question.Content;

    public bool IsMarkDownContent => question.IsMarkDownContent;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => question.Type;
    
    public List<Choice> Choices => question.Choices.Select(choice => new Choice(choice)).ToList();
}