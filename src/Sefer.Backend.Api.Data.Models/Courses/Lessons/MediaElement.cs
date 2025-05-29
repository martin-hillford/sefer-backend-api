namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// A Lesson consists of Element, which can contain text, media, video, YouTube, link, audio etc.
/// They all have in common that they are 'passive' and only present information to the user
/// </summary>
/// <inheritdoc cref="ContentBlock"/>
/// <inheritdoc cref="IContentBlock{TLesson,TContentBlock}"/>
public class MediaElement : ContentBlock, IContentBlock<Lesson, MediaElement>
{
    #region Properties

    /// <summary>
    /// Content for this element (optional).
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Content { get; set; }

    /// <summary>
    /// Holds if the content is mark down content
    /// </summary>
    public bool IsMarkDownContent { get; set; }

    /// <summary>
    /// Reference to external content i.e. a youtube link
    /// </summary>
    [Required, Url, MaxLength(int.MaxValue)]
    public string Url { get; set; }

    /// <summary>
    /// Gets the type of content
    /// </summary>
    [Required]
    public ContentBlockTypes Type { get; set; }

    /// <summary>
    /// The lesson to which the multiple choice question belongs
    /// </summary>
    /// <inheritdoc />
    [ForeignKey("LessonId")]
    public Lesson Lesson { get; set; }

    /// <summary>
    /// Returns a header for this element
    /// </summary>
    /// <inheritdoc />
    public string HeaderText => Element.GetHeaderText(this);

    /// <summary>
    /// The predecessor for this Element
    /// </summary>
    [ForeignKey("PredecessorId")]
    public MediaElement Predecessor { get; set; }

    /// <summary>
    /// Returns if the content block is question
    /// </summary>
    public bool IsQuestion => false;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this element given a lesson
    /// </summary>
    /// <param name="lesson"></param>
    /// <returns></returns>
    /// <inheritdoc />
    public MediaElement CreateSuccessor(Lesson lesson)
    {
        var successor = new MediaElement
        {
            CreationDate = DateTime.UtcNow,
            ForcePageBreak = ForcePageBreak,
            Heading = Heading,
            LessonId = lesson.Id,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            Content = Content,
            Type = Type,
            Url = Url,
            IsMarkDownContent = IsMarkDownContent
        };
        return successor;
    }

    #endregion
}