namespace Sefer.Backend.Api.Data.Models;

/// <summary>
/// This abstract class is for entities that have a creation and modification date
/// </summary>
/// <inheritdoc cref="IModifyDateLogEntity"/>
/// <inheritdoc cref="Entity"/>
public abstract class ModifyDateLogEntity : Entity, IModifyDateLogEntity
{
    /// <summary>
    /// The date the object was created.
    /// </summary>
    /// <inheritdoc />
    [InsertOnly]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The date the object was modified for the last time
    /// </summary>
    /// <inheritdoc />
    public DateTime? ModificationDate { get; set; }
}