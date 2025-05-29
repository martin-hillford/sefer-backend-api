// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Curricula;

/// <summary>
/// A CurriculumRevision represent a certain version of a curriculum
/// </summary>
/// <inheritdoc cref="Revision"/>
/// <inheritdoc cref="IRevision{T}"/>
public class CurriculumRevision : Revision, IRevision<CurriculumRevision>
{
    #region Properties

    /// <summary>
    /// Gets the Curriculum this revision belongs to
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int CurriculumId { get; set; }

    /// <summary>
    /// Get the number of years in this curriculum
    /// </summary>
    /// <remarks>negative not allowed, 0 interpreted as years not used in the curriculum</remarks>
    [Range(0, byte.MaxValue)]
    public byte Years { get; set; }

    /// <summary>
    /// Gets the Curriculum this revision belongs to
    /// </summary>
    [ForeignKey("CurriculumId")]
    public Curriculum Curriculum { get; set; }

    /// <summary>
    /// The revision that is a predecessor of this revision
    /// (Set when a previous revision was promoted)
    /// </summary>
    /// <inheritdoc />
    [ForeignKey("PredecessorId")]
    public CurriculumRevision Predecessor { get; set; }

    /// <summary>
    /// When loaded contains all the blocks in this revision
    /// </summary>
    public ICollection<CurriculumBlock> Blocks { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Since revision can in different stages; this function returns if it's editable
    /// </summary>
    /// <inheritdoc />
    [NotMapped]
    public bool IsEditable => Stage == Stages.Edit || Stage == Stages.Test;

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor for this curriculum revision
    /// </summary>
    /// <returns></returns>
    /// <inheritdoc />
    public CurriculumRevision CreateSuccessor()
    {
        return new CurriculumRevision
        {
            CreationDate = DateTime.UtcNow,
            Stage = Stages.Edit,
            PredecessorId = Id,
            Version = Version + 1,
            CurriculumId = CurriculumId,
            Years = Years,
        };
    }

    #endregion
}