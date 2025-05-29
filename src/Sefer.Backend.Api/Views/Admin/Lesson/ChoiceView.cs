// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.Lesson;

/// <summary>
/// This class is used to post the choice of a multiple choice question
/// </summary>
public class ChoiceView
{
    /// <summary>
    /// The underlying model
    /// </summary>
    private readonly MultipleChoiceQuestionChoice _model;

    /// <summary>
    /// The id of the choice of a multiple choice question
    /// </summary>
    public int Id => _model.Id;

    /// <summary>
    /// Gets / sets the answer of the choice
    /// </summary>
    public string Answer => _model.Answer;

    /// <summary>
    /// Gets / sets if the answer is correct for the choice
    /// </summary>
    [Required]
    public bool IsCorrectAnswer => _model.IsCorrectAnswer;

    /// <summary>
    /// Creates new choice based on a model
    /// </summary>
    /// <param name="model"></param>
    public ChoiceView(MultipleChoiceQuestionChoice model)
    {
        _model = model;
    }
}
