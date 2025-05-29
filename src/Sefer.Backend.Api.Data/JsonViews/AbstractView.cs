namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// Holds an abstract base view, nothing special
/// </summary>
/// <remarks>
/// Creates a new View
/// </remarks>
/// <param name="model">The model of the view</param>
public class AbstractView<TDataContract>(TDataContract model) where TDataContract : IEntity
{
    /// <summary>
    /// The model
    /// </summary>
    protected readonly TDataContract Model = model;

    /// <summary>
    /// The id of the model
    /// </summary>
    public int Id => Model.Id;
}
