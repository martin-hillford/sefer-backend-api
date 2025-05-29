namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// An extension on the IMultipleChoiceQuestionChoice interface within DataContracts
/// </summary>
/// <inheritdoc cref="IEntity"/>
public interface IMultipleChoiceQuestionChoice : IEntity
{
    /// <summary>
    /// Gets / sets the answer of the choice
    /// </summary>
    string Answer { get; set; }

    /// <summary>
    /// Gets / sets if the answer is correct for the choice
    /// </summary>
    bool IsCorrectAnswer { get; set; }

    /// <summary>
    /// Gets / sets the unique identifier of the question to which the choice belongs
    /// </summary>
    int QuestionId { get; set; }
}