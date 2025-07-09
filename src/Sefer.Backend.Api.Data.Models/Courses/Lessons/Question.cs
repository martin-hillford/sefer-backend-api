namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// A Lesson consists of Content which can be either Elements or Questions. This class implements the question part
/// </summary>
/// <inheritdoc cref="ContentBlock" />
public abstract class Question : ContentBlock
{
    /// <summary>
    /// Content for this question, containing the question being asked the student
    /// </summary>
    [Required]
    public string Content { get; set; }

    /// <summary>
    /// Holds if the content is mark-down content
    /// </summary>
    public bool IsMarkDownContent { get; set; } = false;
    
    /// <summary>
    /// A course maker or admin can add an explanation to the question explaining what is the good answer.
    /// This explanation is not shown by default
    /// </summary>
    public string AnswerExplanation { get; set; }
}