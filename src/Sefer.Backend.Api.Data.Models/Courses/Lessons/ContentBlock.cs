namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// Abstract class ContentBlock, is a 'contract' that must implemented by every
/// class that is an in the content of lesson (known: Element and Question)
/// </summary>
/// <inheritdoc cref="IContentBlock" />
/// <inheritdoc cref="ModifyDateLogEntity" />
public abstract class ContentBlock : ModifyDateLogEntity, IContentBlock
{
    #region Properties

    /// <summary>
    /// The id of the lesson to which this element belongs.
    /// </summary>
    /// <inheritdoc />
    [Required]
    [InsertOnly]
    public int LessonId { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this ContentBlock. Does not have to be unique.
    /// But it will be used for sorting the lessons within a Lesson
    /// </summary>
    /// <inheritdoc />
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts (optional).
    /// </summary>
    /// <inheritdoc />
    public string Number { get; set; }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    /// <inheritdoc />
    [Required]
    public bool ForcePageBreak { get; set; }

    /// <summary>
    /// The heading for this element (optional).
    /// </summary>
    /// <inheritdoc />
    public string Heading { get; set; }

    /// <summary>
    /// The element that is a predecessor of this element.
    /// (Set when a previous lesson was promoted)
    /// </summary>
    public int? PredecessorId { get; set; }

    #endregion
}