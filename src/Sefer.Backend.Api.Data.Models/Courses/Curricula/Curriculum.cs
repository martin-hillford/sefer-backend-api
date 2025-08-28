// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Curricula;

/// <summary>
/// A Curriculum is a collection of courses that together aim to educate a user to a certain knowledge level.
/// Curricula can be divided into years, and will always consist of one or more blocks. Curriculum features
/// the same revision  methodology as with Courses.
/// </summary>
/// <remarks>A Curriculum is different from a Series since a Series aims to bundle Courses with a common theme.</remarks>
/// <inheritdoc cref="ModifyDateLogEntity"/>
/// <inheritdoc cref="IRevisionable"/>
/// <inheritdoc cref="IPermaLinkable"/>
public class Curriculum : ModifyDateLogEntity, IRevisionable, IPermaLinkable
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    /// <inheritdoc />
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets the permalink for the curriculum. This should be a unique entry.
    /// </summary>
    /// <inheritdoc />
    [MaxLength(255)]
    public string Permalink { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [Required]
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the curriculum
    /// </summary>
    [Required]
    public Levels Level { get; set; }

    /// <summary>
    /// The several revision of this curriculum
    /// </summary>
    [InverseProperty("Curriculum")]
    public ICollection<CurriculumRevision> Revisions { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Return the editing curriculum revision for this curriculum
    /// </summary>
    [NotMapped]
    public CurriculumRevision EditingCurriculumRevision
    {
        get
        {
            var revisions = from rev in Revisions
                            where rev.Stage == Stages.Edit || rev.Stage == Stages.Test
                            select rev;
            return revisions.FirstOrDefault();
        }
    }

    /// <summary>
    /// Return the published curriculum revision for this curriculum
    /// </summary>
    [NotMapped]
    public CurriculumRevision PublishedCurriculumRevision
    {
        get
        {
            var revisions = from rev in Revisions
                            where rev.Stage == Stages.Published
                            select rev;
            return revisions.FirstOrDefault();
        }
    }

    /// <summary>
    /// Returns if the curriculum is published
    /// </summary>
    public bool IsPublished => PublishedCurriculumRevision != null;

    /// <summary>
    /// Returns all the closed revisions for the curriculum
    /// </summary>
    [NotMapped]
    public IEnumerable<CurriculumRevision> ClosedRevisions
    {
        get
        {
            var revisions = from rev in Revisions
                            where rev.Stage == Stages.Closed
                            select rev;
            return revisions;
        }
    }

    /// <summary>
    /// returns the overall stage of this curriculum
    /// Published: the curriculum has a published revision
    /// Closed: the curriculum has an edit revision and one or closed revisions but no published revision
    /// Edit: the curriculum has only an edit revision but no published or closed one.
    /// </summary>
    /// <returns>the stage of which this whole course is in.</returns>
    [NotMapped]
    public Stages OverallStage
    {
        get
        {
            if (Revisions.Count == 0) return Stages.Edit;
            if (PublishedCurriculumRevision != null) return Stages.Published;
            if (ClosedRevisions.Any()) return Stages.Closed;
            return EditingCurriculumRevision?.Stage ?? Stages.Edit;
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

    /// <summary>
    /// Returns a list of all the published courses in this curriculum
    /// </summary>
    public List<Course> GetPublishedCourses()
    {
        if (PublishedCurriculumRevision == null) return new();
        return PublishedCurriculumRevision.Blocks.SelectMany(b => b.Courses).ToList();
    }

    #endregion
}