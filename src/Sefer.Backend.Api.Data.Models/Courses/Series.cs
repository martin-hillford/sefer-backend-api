// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// The Series class represents a collection of courses with a name a description.
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class Series : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    [Required]
    [MaxLength(255)]
    [MinLength(3)]
    public string Name { get; set; }

    
    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    /// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
    [Required]
    public string Description { get; set; }

    /// <summary>
    /// The difficulty level of the course
    /// </summary>
    [Required]
    public Levels Level { get; set; }

    /// <summary>
    /// When this is set to true, the Series will be displayed for users
    /// (students and visitors on the website)
    /// </summary>
    [Required]
    public bool IsPublic { get; set; }

    /// <summary>
    /// This is the sort index used for determining the sort order
    /// </summary>
    public int SequenceId { get; set; }

    #endregion

    #region References

    /// <summary>
    /// Gets or sets the Courses in this Series
    /// </summary>
    public ICollection<SeriesCourse> SeriesCourses { get; set; }

    #endregion
}