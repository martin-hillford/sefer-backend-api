// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global UnusedMember.Global
namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A view on the lessons (within a revision)
/// </summary>
/// <inheritdoc cref="AbstractView{Lesson}"/>
public class LessonView
{
    #region Properties

    /// <summary>
    /// The id of the model
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The date the object was created
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The date the object was modified for the last time
    /// </summary>
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Gets / Sets the sequence number for this lesson. Does not have to be unique.
    /// But it will be used for sorting the lessons within a course revision
    /// </summary>
    public int SequenceId { get; set; }

    /// <summary>
    /// Gets / sets the chapter / section number for this lesson
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    ///  Gets / sets name for this lesson
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Returns a simple header for the lesson (western languages)
    /// </summary>
    public string Header => Number + ":&nbsp;" + Name;

    /// <summary>
    /// Gets / sets a description for this lesson
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets / sets what the student should read before the start of the lesson
    /// </summary>
    public string ReadBeforeStart { get; set; }

    /// <summary>
    /// Gets / sets the id of the <see cref="CourseRevision"/>  this lesson belongs to
    /// </summary>
    public int CourseRevisionId { get; set; }

    /// <summary>
    /// Get or set the id of the predecessor
    /// </summary>
    public int? PredecessorId { get; set; }

    /// <summary>
    /// Holds if this lesson has 'text to speech' functionality so
    /// that the user also can listen to this lesson's content.
    /// </summary>
    public Guid? AudioReferenceId { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="lesson">The model of the view</param>
    public LessonView(Lesson lesson)
    {
        PredecessorId = lesson.PredecessorId;
        CourseRevisionId = lesson.CourseRevisionId;
        Description = lesson.Description;
        ReadBeforeStart = lesson.ReadBeforeStart;
        Name = lesson.Name;
        Number = lesson.Number;
        SequenceId = lesson.SequenceId;
        ModificationDate = lesson.ModificationDate;
        CreationDate = lesson.CreationDate;
        Id = lesson.Id;
        AudioReferenceId = lesson.AudioReferenceId;
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    public LessonView() { }

    #endregion
}
