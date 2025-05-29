namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// Defines a choice a user can make to answer the question
/// </summary>
public class SurveyMultipleChoiceQuestionChoice : Entity, IMultipleChoiceQuestionChoice
{
    #region Properties

    /// <summary>
    /// Gets / sets the answer of the choice
    /// </summary>
    [Required, MaxLength(int.MaxValue)]
    public string Answer { get; set; }

    /// <summary>
    /// Gets / sets if the answer is correct for the choice
    /// </summary>
    [Required]
    public bool IsCorrectAnswer { get; set; }

    /// <summary>
    /// Gets / sets the unique identifier of the question to which the choice belongs
    /// </summary>
    [Required]
    public int QuestionId { get; set; }

    /// <summary>
    /// Gets / sets the question to which the choice belongs
    /// </summary>
    [ForeignKey("QuestionId")]
    public SurveyMultipleChoiceQuestion Question { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Create a successor for the lesson given a question the successor will belong
    /// </summary>
    /// <param name="question"></param>
    /// <returns></returns>
    public SurveyMultipleChoiceQuestionChoice CreateSuccessor(SurveyMultipleChoiceQuestion question)
    {
        return new SurveyMultipleChoiceQuestionChoice
        {
            Answer = Answer,
            IsCorrectAnswer = IsCorrectAnswer,
            QuestionId = question.Id,
        };
    }

    #endregion
}