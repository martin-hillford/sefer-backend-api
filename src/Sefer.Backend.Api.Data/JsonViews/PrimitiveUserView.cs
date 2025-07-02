namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A very primitive view on user, use when personal information of user is not required
/// </summary>
public class PrimitiveUserView
{
    /// <summary>
    /// The id of the user
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the user
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Creates a new view on a user
    /// </summary>
    /// <param name="user">The user to create a view for</param>
    public PrimitiveUserView(User user)
    {
        Id = user.Id;
        Name = user.Name;
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    [JsonConstructor]
    public PrimitiveUserView() { }
}