// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Models.Public.Download;

public class Choice(MultipleChoiceQuestionChoice choice)
{
    public string Answer => choice.Answer;
    
    public bool IsCorrectAnswer => choice.IsCorrectAnswer;

    public int Id => choice.Id;
}