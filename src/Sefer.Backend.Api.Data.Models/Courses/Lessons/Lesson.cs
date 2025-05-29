// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Lessons;

/// <summary>
/// Class Lesson handles with a single lesson a student can take. One per day.
/// </summary>
/// <inheritdoc cref="ILesson"/>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class Lesson : ModifyDateLogEntity, ILesson
{
    #region Properties

    /// <summary>
    /// Gets / Sets the sequence number for this lesson. Does not have to be unique.
    /// But it will be used for sorting the lessons within a course revision
    /// </summary>
    [Required]
    public int SequenceId { get; set; }

    /// <summary>
    /// Gets / sets the chapter / section number for this lesson
    /// </summary>
    [Required]
    [MaxLength(50)]
    [MinLength(1)]
    public string Number { get; set; }

    /// <summary>
    ///  Gets / sets name for this lesson
    /// </summary>
    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets a description for this lesson
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Description { get; set; }

    /// <summary>
    /// Gets / sets what the student should read before the start of the lesson
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string ReadBeforeStart { get; set; }

    /// <summary>
    /// Gets / sets the id of the <see cref="Sefer.Backend.Api.Data.Models.Courses.CourseRevision"/>  this lesson belongs to
    /// </summary>
    public int CourseRevisionId { get; set; }

    /// <summary>
    /// Get or set the id of the predecessor
    /// </summary>
    public int? PredecessorId { get; set; }

    /// <summary>
    /// Gets / sets to which this revision belongs
    /// </summary>
    [ForeignKey("CourseRevisionId")]
    public CourseRevision CourseRevision { get; set; }

    /// <summary>
    /// The Lesson that is a predecessor of this Lesson.
    /// (Set when a previous course revision was promoted)
    /// </summary>
    public Lesson Predecessor { get; set; }

    /// <summary>
    /// A collection with content (should be set using a repository)
    /// </summary>
    [NotMapped]
    public IEnumerable<IContentBlock<Lesson>> Content { get; set; }


    /// <summary>
    /// Returns a simple header for the lesson (western languages)
    /// </summary>
    [NotMapped]
    public string Header => Number + ": " + Name;

    /// <summary>
    /// Holds if this lesson has 'text to speech' functionality so
    /// that the user also can listen to this lesson's content.
    /// </summary>
    public Guid? AudioReferenceId { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new empty lesson
    /// </summary>
    public Lesson() { Content = []; }

    #endregion

    #region Methods

    /// <summary>
    /// Create a successor for the lesson given a revision
    /// </summary>
    /// <param name="revision"></param>
    /// <returns></returns>
    public Lesson CreateSuccessor(CourseRevision revision)
    {
        // Nb. also copy the audio reference id. The upload functionality
        // will ensure that the reference is correctly handled. It would
        // be pointless to keep on duplicating audio
        return new Lesson
        {
            CourseRevisionId = revision.Id,
            CreationDate = DateTime.UtcNow,
            Description = Description,
            ReadBeforeStart = ReadBeforeStart,
            Name = Name,
            Number = Number,
            PredecessorId = Id,
            SequenceId = SequenceId,
            AudioReferenceId = AudioReferenceId
        };
    }

    public Question GetQuestion(QuestionAnswer answer)
        => Content.First(c => c.Id == answer.QuestionId && c.Type == answer.QuestionType) as Question;

    #endregion
}