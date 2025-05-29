namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// An open question is just a question with a question which the user has
/// to respond with a written text.
/// </summary>
/// <inheritdoc cref="SurveyQuestion"/>
public class SurveyOpenQuestion : SurveyQuestion, ISurveyQuestion<Survey, SurveyOpenQuestion>
{
    #region Properties

    /// <summary>
    /// The survey to which the multiple choice question belongs
    /// </summary>
    [ForeignKey("SurveyId")]
    public Survey Survey { get; set; }

    /// <summary>
    /// Returns a header for this open question
    /// </summary>
    /// <inheritdoc />
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// The predecessor for this OpenQuestion
    /// </summary>
    [ForeignKey("PredecessorId")]
    public SurveyOpenQuestion Predecessor { get; set; }

    #endregion

    #region Method

    /// <summary>
    /// Creates a successor for this element given a survey
    /// </summary>
    /// <param name="survey"></param>
    /// <returns></returns>
    public SurveyOpenQuestion CreateSuccessor(Survey survey)
    {
        var successor = new SurveyOpenQuestion
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            SurveyId = Survey.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            Text = Text
        };
        return successor;
    }

    #endregion
}