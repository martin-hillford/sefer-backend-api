namespace Sefer.Backend.Api.Data.Models;

/// <summary>
/// Base class for all entities in the system
/// </summary>
/// <inheritdoc cref="IEntity"/>
public abstract class Entity : IEntity
{
    /// <summary>
    /// The only thing required from an entity is it uniquely identifiable
    /// </summary>
    /// <inheritdoc cref="IEntity.Id"/>
    [Key]
    public int Id { get; set; }
}