// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Lessons;

/// <summary>
/// This class is used to post the choice of a multiple choice question
/// </summary>
public sealed class ChoiceView
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
    /// Creates new choice based on a model
    /// </summary>
    /// <param name="model"></param>
    public ChoiceView(MultipleChoiceQuestionChoice model)
    {
        _model = model;
    }
}