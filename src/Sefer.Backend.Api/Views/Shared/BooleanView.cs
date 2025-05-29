namespace Sefer.Backend.Api.Views.Shared;

/// <summary>
/// A shared view to response with a boolean, to be used in example with remote validation method
/// </summary>
public class BooleanView
{
    /// <summary>
    /// The response 
    /// </summary>
    public bool Response { get; init; }

    /// <summary>
    /// Creates a new boolean view
    /// </summary>
    public BooleanView() { }

    /// <summary>
    /// Creates a new boolean view
    /// </summary>
    /// <param name="response">The boolean response</param>
    public BooleanView(bool response)
    {
        Response = response;
    }
}