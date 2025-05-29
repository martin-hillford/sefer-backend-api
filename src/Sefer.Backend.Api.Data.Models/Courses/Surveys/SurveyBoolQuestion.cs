namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// Defines a simple yes or no (true or false) question
/// </summary>
/// <inheritdoc cref="SurveyQuestion" />
/// <inheritdoc cref="ISurveyQuestion{TSurvey,TQuestion}"/>
public class SurveyBoolQuestion : SurveyQuestion, ISurveyQuestion<Survey, SurveyBoolQuestion>
{
    #region Properties

    /// <summary>
    /// Get or set if the correct answer for the question is true
    /// </summary>
    public bool CorrectAnswerIsTrue { get; set; }

    /// <summary>
    /// Get or set if the correct answer for the question is true
    /// </summary>
    [NotMapped]
    public BoolAnswers CorrectAnswer
    {
        get => CorrectAnswerIsTrue ? BoolAnswers.Correct : BoolAnswers.Wrong;
        set => CorrectAnswerIsTrue = value == BoolAnswers.Correct;
    }

    /// <summary>
    /// The survey to which the multiple choice question belongs
    /// </summary>
    [ForeignKey("SurveyId")]
    public Survey Survey { get; set; }

    /// <summary>
    /// Returns a header for this open question
    /// </summary>
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// The predecessor for this BoolQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public SurveyBoolQuestion Predecessor { get; set; }

    #endregion

    #region Method

    /// <summary>
    /// Creates a successor for this element given a survey
    /// </summary>
    /// <param name="survey"></param>
    /// <returns></returns>
    public SurveyBoolQuestion CreateSuccessor(Survey survey)
    {
        var successor = new SurveyBoolQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            SurveyId = Survey.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            Text = Text,
            CorrectAnswer = CorrectAnswer,
        };
        return successor;
    }

    #endregion
}