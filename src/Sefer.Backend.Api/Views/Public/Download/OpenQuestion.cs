// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global
namespace Sefer.Backend.Api.Views.Public.Download;

public class OpenQuestion(Data.Models.Courses.Lessons.OpenQuestion openQuestion) : ContentBlock(openQuestion)
{
    public override string Content { get; set; } = openQuestion.Content;

    public bool IsMarkDownContent => openQuestion.IsMarkDownContent;
    
    public string AnswerExplanation => openQuestion.AnswerExplanation;
    
    public string ExactAnswer => openQuestion.ExactAnswer;
    
    public bool IsGradable => openQuestion.IsGradable;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => openQuestion.Type;
}