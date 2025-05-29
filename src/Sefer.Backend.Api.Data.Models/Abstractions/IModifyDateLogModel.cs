namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// This interface is for entities that have a creation and modification date
/// </summary>
/// <inheritdoc />
public interface IModifyDateLogEntity : IEntity
{
    /// <summary>
    /// The date the object was created.
    /// </summary>
    DateTime CreationDate { get; set; }

    /// <summary>
    /// The date the object was modified for the last time
    /// </summary>
    DateTime? ModificationDate { get; set; }
}