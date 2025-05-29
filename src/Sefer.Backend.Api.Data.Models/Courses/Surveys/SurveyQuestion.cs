namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// A Lesson consists of Content which can be either Elements or Questions. This class implements the question part
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity" />
public abstract class SurveyQuestion : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// Gets the survey to which this entity belongs
    /// </summary>
    public int SurveyId { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this Question. Does not have to be unique.
    /// But it will be used for sorting the lessons within a Lesson
    /// </summary>
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// Text for this question, containing the question asked the student
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// A number for this question
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    [Required]
    public bool ForcePageBreak { get; set; }

    /// <summary>
    /// A heading for this question (optional).
    /// </summary>
    public string Heading { get; set; }

    /// <summary>
    /// The element that is a predecessor of this element.
    /// (Set when a previous survey was promoted)
    /// </summary>
    public int? PredecessorId { get; set; }

    #endregion
}