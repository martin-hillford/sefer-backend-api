namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Some objects will be revisionable, using a revision system
/// </summary>
/// <inheritdoc />
public interface IRevision<T> : IRevision where T : IRevision<T>
{
    #region Properties

    /// <summary>
    /// The revision that is a predecessor of this revision
    /// (Set when a previous revision was promoted)
    /// </summary>
    T Predecessor { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Since revision can in different stages; this function returns if it's editable
    /// </summary>
    bool IsEditable { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a clone of this model
    /// </summary>
    /// <returns></returns>
    T CreateSuccessor();

    #endregion
}

/// <summary>
/// This interface defines the properties for individual revisions of an IRevisionable
/// </summary>
/// <inheritdoc />
public interface IRevision : IModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The stage of the revision
    /// </summary>
    Stages Stage { get; set; }

    /// <summary>
    /// The version of the revision (increasing number)
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Get or set the id of the predecessor
    /// </summary>
    int? PredecessorId { get; set; }

    #endregion
}