namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Basic interface defining a highly abstract Entity
/// </summary>
public interface IEntity
{
    /// <summary>
    /// The only thing required from an entity is it uniquely identifiable
    /// </summary>
    int Id { get; set;  }
}