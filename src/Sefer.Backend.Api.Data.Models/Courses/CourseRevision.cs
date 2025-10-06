// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// The Revision of a Course
/// </summary>
/// <remarks>
/// Courses and their content will change over time. However, for students it will be a problem
/// if the content of the course they followed is not available because an update changed the
/// content. Therefore, Courses feature a CourseRevision. A Course will always have 1
/// 'EditingRevision'. This Revision is in either the Edit or Test stage. This revision can be
/// modified and edited by a CourseMaker or by an Administrator. When an EditingRevision is
/// ready, the Administrator can publish it to become the 'PublishedRevision'. The publishing
/// process will close the current PublishedRevision, promote the current EditingRevision to be
/// the PublishedRevision and will create a new revision as the EditingRevision.The Administrator
/// can also close the PublishedRevision. Only the PublishedRevision is available
/// </remarks>
/// <inheritdoc cref="Sefer.Backend.Api.Data.Models.Courses.CourseRevision"/>
/// <inheritdoc cref="IRevision{T}"/>
public class CourseRevision : Revision, IRevision<CourseRevision>
{
    #region Properties

    /// <summary>
    /// When this is set to true, Students are allowed to take this course without the aid of a mentor
    /// </summary>
    [Required]
    public bool AllowSelfStudy { get; set; }
    
    /// <summary>
    /// Contains general course information that can be displayed with each lesson
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string GeneralInformation { get; set; }
    
    /// <summary>
    /// The enrollments that students have on this course revision
    /// </summary>
    [InverseProperty("CourseRevision")]
    public ICollection<Enrollment> Enrollments { get; set; }

    /// <summary>
    /// Gets the CourseId of this revision
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    /// <summary>
    /// Gets or set the identifier of the survey for this lesson
    /// </summary>
    public int? SurveyId { get; set; }

    #endregion

    #region References

    /// <summary>
    /// Gets the course this revision belongs to
    /// </summary>
    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    /// <summary>
    /// Gets all the lessons for the CourseRevision
    /// </summary>
    public ICollection<Lesson> Lessons { get; set; }
    
    /// <summary>
    /// Get or sets the survey for this lesson
    /// </summary>
    public Survey Survey { get; set; }

    /// <summary>
    /// The revision that is a predecessor of this revision
    /// (Set when a previous revision was promoted)
    /// </summary>
    /// <inheritdoc />
    [ForeignKey("PredecessorId")]
    public CourseRevision Predecessor { get; set; }
    
    /// <summary>
    /// Contains a set of dictionary words that are as a small dictionary to this course
    /// </summary>
    [InverseProperty(nameof(CourseRevisionDictionaryWord.CourseRevision))]
    public List<CourseRevisionDictionaryWord> DictionaryWords { get; set; }

    /// <summary>
    /// Checks if the course is editable, which depends on the stage of the CourseRevision
    /// </summary>
    /// <returns>True when the course is editable else false</returns>
    /// <inheritdoc />
    [NotMapped]
    public bool IsEditable => Stage is Stages.Edit or Stages.Test;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a clone of this model
    /// </summary>
    /// <returns></returns>
    /// <inheritdoc />
    public CourseRevision CreateSuccessor()
    {
        var successor = new CourseRevision
        {
            AllowSelfStudy = AllowSelfStudy,
            CourseId = CourseId,
            CreationDate = DateTime.UtcNow,
            GeneralInformation = GeneralInformation,
            Stage = Stages.Edit,
            PredecessorId = Id,
            Version = Version + 1,
        };
        return successor;
    }

    #endregion
}