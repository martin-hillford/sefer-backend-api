namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// Defines a choice a user can make to answer the question
/// </summary>
/// <inheritdoc cref="Entity" />
/// <inheritdoc cref="IMultipleChoiceQuestionChoice" />
public class MultipleChoiceQuestionChoice : Entity, IMultipleChoiceQuestionChoice
{
    #region Properties

    /// <summary>
    /// Gets / sets the answer of the choice
    /// </summary>
    /// <inheritdoc />
    [Required, MaxLength(int.MaxValue)]
    public string Answer { get; set; }

    /// <summary>
    /// Gets / sets if the answer is correct for the choice
    /// </summary>
    /// <inheritdoc />
    [Required]
    public bool IsCorrectAnswer { get; set; }

    /// <summary>
    /// Gets / sets the unique identifier of the question to which the choice belongs
    /// </summary>
    /// <inheritdoc />
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// Gets / sets the question to which the choice belongs
    /// </summary>
    [ForeignKey("QuestionId")]
    public MultipleChoiceQuestion Question { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Create a successor for the lesson given a question the successor will belong
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    public MultipleChoiceQuestionChoice CreateSuccessor(MultipleChoiceQuestion question)
    {
        return new MultipleChoiceQuestionChoice
        {
            Answer = Answer,
            IsCorrectAnswer = IsCorrectAnswer,
            QuestionId = question.Id,
        };
    }

    #endregion
}