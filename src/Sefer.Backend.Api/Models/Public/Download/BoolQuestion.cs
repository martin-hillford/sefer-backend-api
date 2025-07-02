// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class BoolQuestion(Data.Models.Courses.Lessons.BoolQuestion boolQuestion) : ContentBlock(boolQuestion)
{
    public string Content => boolQuestion.Content;
    
    public bool IsMarkDownContent => boolQuestion.IsMarkDownContent;
    
    public bool CorrectAnswerIsTrue => boolQuestion.CorrectAnswerIsTrue;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentBlockTypes Type => boolQuestion.Type;
}