// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

public sealed class Course : ModifyDateLogEntity, IPermaLinkable, IRevisionable
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    /// <inheritdoc />
    [MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets the permalink for the course. This should be a unique entry.
    /// </summary>
    /// <inheritdoc />
    [PermalinkFormat, MinLength(1)]
    public string Permalink { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The author who wrote the course
    /// </summary>
    [MaxLength(255)]
    public string Author { get; set; }

    /// <summary>
    /// The author who wrote the course
    /// </summary>
    [MaxLength(255)]
    public string Citation { get; set; }

    /// <summary>
    /// A link to the webshop where the course can be ordered on paper
    /// </summary>
    public string WebshopLink { get; set; }

    /// <summary>
    /// A link to a YouTube or other movie that introduces the course
    /// </summary>
    public string IntroductionLink { get; set; }

    /// <summary>
    /// A notice about who has the copyright of this course
    /// </summary>
    public string Copyright { get; set; }

    /// <summary>
    /// A full https link to a logo to be shown with the copyright notice
    /// </summary>
    [MaxLength(255)]
    public string CopyrightLogo { get; set; }

    /// <summary>
    /// The maximum number of lesson to submit per day for this course per student
    /// </summary>
    public int? MaxLessonSubmissionsPerDay { get; set; }

    /// <summary>
    /// The difficulty level of the course
    /// </summary>
    public Levels Level { get; set; }

    /// <summary>
    /// Indicates if this course is a video based or a text based course
    /// </summary>
    public bool IsVideoCourse { get; set; }

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// Gets the HeaderImage for the course. Setting of this image  will be via a file upload.
    /// </summary>
    public string LargeImage { get; set; }

    /// <summary>
    /// Gets the Thumbnail image for the course. Setting of this image will be via a file upload.
    /// </summary>
    public string ThumbnailImage { get; set; }

    #endregion

    #region References

    /// <summary>
    /// When this is set to true, the Course will be displayed on the homepage.
    /// Of course only when the Course has a published Revision!
    /// </summary>
    [Required]
    public bool ShowOnHomepage
    {
        get => _showOnHomepage;
        set
        {
            if (value) { _private = false; }
            _showOnHomepage = value;
        }
    }

    /// <summary>
    /// Real field for the show on homepage value
    /// </summary>
    private bool _showOnHomepage;

    /// <summary>
    /// When this is set to true, the course will be a private course. And will not show up somewhere
    /// That is User are able to take the course using a special url for the course in conjunction with a mentor
    /// </summary>
    [Required]
    public bool Private
    {
        get => _private;
        set
        {
            if (value) { _showOnHomepage = false; }
            _private = value;
        }
    }

    /// <summary>
    /// Real field for the private value
    /// </summary>
    private bool _private;

    /// <summary>
    /// Get or set required courses for this course
    /// </summary>
    [InverseProperty("Course")]
    public ICollection<CoursePrerequisite> Prerequisites { get; set; }

    /// <summary>
    /// Get or set required courses for this course
    /// </summary>
    [InverseProperty("RequiredCourse")]
    public ICollection<CoursePrerequisite> RequiredFor { get; set; }

    /// <summary>
    /// Gets or set the collection of course revision for this course
    /// </summary>
    [InverseProperty("Course")]
    public ICollection<CourseRevision> CourseRevisions { get; set; }

    /// <summary>
    /// A collection to all the mentor of this course (join table)
    /// </summary>
    [InverseProperty("Course")]
    public ICollection<MentorCourse> Mentors { get; set; }

    /// <summary>
    /// Gets all the series courses to which this course belongs
    /// </summary>
    [InverseProperty("Course")]
    public ICollection<SeriesCourse> SeriesCourses { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Return the editing course revision for this courses
    /// </summary>
    [NotMapped]
    public CourseRevision EditingCourseRevision
    {
        get
        {
            var revisions = from rev in CourseRevisions
                            where rev.Stage == Stages.Edit || rev.Stage == Stages.Test
                            select rev;
            return revisions.FirstOrDefault();
        }
    }

    /// <summary>
    /// Return the published course revision for this courses
    /// </summary>
    [NotMapped]
    public CourseRevision PublishedCourseRevision
    {
        get
        {
            var revisions = from rev in CourseRevisions
                            where rev.Stage == Stages.Published
                            select rev;
            return revisions.FirstOrDefault();
        }
    }

    /// <summary>
    /// Returns if the course is published
    /// </summary>
    public bool IsPublished => PublishedCourseRevision != null;

    /// <summary>
    /// Returns all the closed revisions for the course
    /// </summary>
    [NotMapped]
    public IEnumerable<CourseRevision> ClosedRevisions
    {
        get
        {
            var revisions = from rev in CourseRevisions
                            where rev.Stage == Stages.Closed
                            select rev;
            return revisions;
        }
    }

    /// <summary>
    /// returns the overall stage of this course
    /// Published: the course has a published revision
    /// Closed: the course has an edit revision and one or closed revisions but no published revision
    /// Edit: the course has only an edit revision but no published or closed one.
    /// </summary>
    /// <returns>the stage of which this whole course is in.</returns>
    [NotMapped]
    public Stages? OverallStage
    {
        get
        {
            if (CourseRevisions == null) return null;
            if (CourseRevisions.Any() == false) return Stages.Edit;
            if (PublishedCourseRevision != null) return Stages.Published;
            if (ClosedRevisions.Any()) return Stages.Closed;
            return EditingCourseRevision?.Stage ?? Stages.Edit;
        }
    }

    /// <summary>
    /// Checks if the course is editable which is depending on the stage of the CourseRevision
    /// </summary>
    /// <returns>True when the course is editable else false</returns>
    [NotMapped]
    public bool IsEditable
    {
        get
        {
            var stage = OverallStage;
            return stage is Stages.Edit or Stages.Test;
        }
    }

    #endregion
}