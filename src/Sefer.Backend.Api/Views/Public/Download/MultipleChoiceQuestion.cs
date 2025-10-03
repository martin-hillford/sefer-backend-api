// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global

using Sefer.Backend.Api.Models.Public;

namespace Sefer.Backend.Api.Views.Public.Download;

public class MultipleChoiceQuestion(Data.Models.Courses.Lessons.MultipleChoiceQuestion question) : ContentBlock(question)
{
    public override string Content { get; set; } = question.Content;

    public bool IsMarkDownContent => question.IsMarkDownContent;
    
    public bool IsMultiSelect => question.IsMultiSelect;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => question.Type;
    
    public List<Choice> Choices => question.Choices.Select(choice => new Choice(choice)).ToList();
    
    public string AnswerExplanation => question.AnswerExplanation;
    
    public override async Task IncludeMedia(DownloadRequest request, Course course)
    {
        await base.IncludeMedia(request, course);
        foreach (var choice in Choices)
        {
            await choice.IncludeMedia(request, course);
        }
    }
}