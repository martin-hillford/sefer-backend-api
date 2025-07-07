// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable MemberCanBePrivate.Global, UnusedMember.Global
namespace Sefer.Backend.Api.Views.Public.Download;

public class BoolQuestion(Data.Models.Courses.Lessons.BoolQuestion boolQuestion) : ContentBlock(boolQuestion)
{
    public override string Content { get; set; } = boolQuestion.Content;
    
    public bool IsMarkDownContent => boolQuestion.IsMarkDownContent;
    
    public bool CorrectAnswerIsTrue => boolQuestion.CorrectAnswerIsTrue;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => boolQuestion.Type;
}