namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// Some objects will be revisionable, using a revision system
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
/// <inheritdoc cref="IRevision"/>
public abstract class Revision : ModifyDateLogEntity, IRevision
{
    #region Properties

    /// <summary>
    /// The stage of the revision
    /// </summary>
    /// <inheritdoc />
    [Required]
    public Stages Stage { get; set; }

    /// <summary>
    /// The version of the revision (increasing number)
    /// </summary>
    /// <inheritdoc />
    [Required]
    public int Version { get; set; }

    /// <summary>
    /// Get or set the id of the predecessor
    /// </summary>
    /// <inheritdoc />
    public int? PredecessorId { get; set; }

    #endregion
}