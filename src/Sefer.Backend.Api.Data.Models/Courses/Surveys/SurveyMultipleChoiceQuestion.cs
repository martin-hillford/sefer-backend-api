namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// A Multiple Choice Question is a question for which the user can select one or more
/// answer as being the correct answer
/// </summary>
/// <inheritdoc cref="SurveyQuestion"/>
/// <inheritdoc cref="IMultipleChoiceQuestion{T}"/>
/// <inheritdoc cref="ISurveyQuestion{Survey, MultipleChoiceQuestion}"/>
public class SurveyMultipleChoiceQuestion : SurveyQuestion, IMultipleChoiceQuestion<SurveyMultipleChoiceQuestionChoice>, ISurveyQuestion<Survey, SurveyMultipleChoiceQuestion>
{
    #region Properties

    /// <summary>
    /// Gets / sets if the user can select multiple choices as being correct
    /// </summary>
    /// <inheritdoc />
    [Required]
    [MultiSelectQuestion]
    public bool IsMultiSelect { get; set; }

    /// <summary>
    /// Contains a list of right answer
    /// </summary>
    [Count(2)]
    public ICollection<SurveyMultipleChoiceQuestionChoice> Choices { get; set; }

    /// <summary>
    /// The Survey to which the multiple choice question belongs
    /// </summary>
    [ForeignKey("SurveyId")]
    public Survey Survey { get; set; }

    /// <summary>
    /// Gets the number of correct answers for this  multiple choice collection
    /// </summary>
    /// <inheritdoc />
    [NotMapped]
    public int CorrectAnswerCount => (from choice in Choices where choice.IsCorrectAnswer select choice).Count();

    /// <summary>
    /// Returns a header for this question
    /// </summary>
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// The predecessor for this MultipleChoiceQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public SurveyMultipleChoiceQuestion Predecessor { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this element given a Survey
    /// (Does not include choices!!)
    /// </summary>
    /// <param name="survey"></param>
    /// <returns></returns>
    public SurveyMultipleChoiceQuestion CreateSuccessor(Survey survey)
    {
        var successor = new SurveyMultipleChoiceQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            SurveyId = survey.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            IsMultiSelect = IsMultiSelect,
            Text = Text
        };
        return successor;
    }

    #endregion
}