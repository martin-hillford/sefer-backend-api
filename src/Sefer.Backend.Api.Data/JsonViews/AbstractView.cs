namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// Holds an abstract base view, nothing special
/// </summary>
/// <remarks>
/// Creates a new View
/// </remarks>
/// <param name="model">The model of the view</param>
public class AbstractView<TDataContract> where TDataContract : IEntity
{
    /// <summary>
    /// The model
    /// </summary>
    protected TDataContract Model;

    /// <summary>
    /// The id of the model
    /// </summary>
    public int Id { get; set; }

    protected AbstractView(TDataContract model)
    {
        Model = model;
        Id =  Model.Id;
    }
    
    [JsonConstructor]
    public AbstractView() { }
}
