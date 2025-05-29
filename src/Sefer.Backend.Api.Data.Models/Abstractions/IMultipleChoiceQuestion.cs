namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// This interface will be used in conjunction with a validator to test
/// for the multiple choice selection and multiple correct answers
/// </summary>
public interface IMultipleChoiceQuestion : IEntity
{
    #region Properties

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct
    /// </summary>
    bool IsMultiSelect { get; set; }

    /// <summary>
    /// Gets the number of correct answers for this  multiple choice collection
    /// </summary>
    int CorrectAnswerCount { get; }

    #endregion
}

public interface IMultipleChoiceQuestion<T> : IMultipleChoiceQuestion where T : IMultipleChoiceQuestionChoice
{
    /// <summary>
    /// The choices for this question
    /// </summary>
    ICollection<T> Choices { get; set; }
}