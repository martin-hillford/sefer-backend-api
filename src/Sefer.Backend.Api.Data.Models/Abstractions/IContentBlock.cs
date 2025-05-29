namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Abstract class ContentBlock, is a 'contract' that must implemented by every
/// class that is an in the content of lesson (known: Element and Question)
/// </summary>
/// <inheritdoc cref="IContentBlock{TLesson}"/>
public interface IContentBlock<TLesson, out TContentBlock> : IContentBlock<TLesson>
    where TLesson : ILesson
    where TContentBlock : IContentBlock<TLesson, TContentBlock>
{
    /// <summary>
    /// Creates a successor for this TContentBlock given a lesson
    /// </summary>
    /// <param name="lesson">The lesson to use</param>
    /// <returns>a new TContentBlock succeeding this</returns>
    TContentBlock CreateSuccessor(TLesson lesson);
}

/// <summary>
/// And a less strict interface for more general purposes
/// </summary>
/// <typeparam name="TLesson"></typeparam>
/// <inheritdoc cref="IElement"/>
public interface IContentBlock<TLesson> : IElement where TLesson : ILesson
{
    /// <summary>
    /// The lesson to which this element belongs.
    /// </summary>
    TLesson Lesson { get; set; }

    /// <summary>
    /// The id of the lesson to which this element belongs.
    /// </summary>
    int LessonId { get; set; }

    /// <summary>
    /// To help with abstracting, each class implementing ContentBlock
    /// should return a type indicator of how to render the Block when
    /// provided in an abstract way.
    /// </summary>
    ContentBlockTypes Type { get; }

    /// <summary>
    /// The element that is a predecessor of this element.
    /// (Set when a previous lesson was promoted)
    /// </summary>
    int? PredecessorId { get; set; }

    /// <summary>
    /// Returns if the content block is question
    /// </summary>
    bool IsQuestion { get; }
}

/// <summary>
/// Abstract class ContentBlock, is a 'contract' that must implemented by every
/// class that is an in the content of lesson (known: Element and Question)
/// </summary>
/// <inheritdoc />
public interface IContentBlock : IModifyDateLogEntity
{
    /// <summary>
    /// The id of the lesson to which this element belongs.
    /// </summary>
    int LessonId { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this ContentBlock. Does not have to be unique.
    /// But it will be used for sorting the lessons within a Lesson
    /// </summary>
    int SequenceId { get; set; }

    /// <summary>
    /// A number for this element. Can be used to separate a lesson in parts (optional).
    /// </summary>
    string Number { get; set; }

    /// <summary>
    /// When true, a page break will be forced for the user
    /// </summary>
    bool ForcePageBreak { get; set; }

    /// <summary>
    /// The heading for this element (optional).
    /// </summary>
    string Heading { get; set; }
}
