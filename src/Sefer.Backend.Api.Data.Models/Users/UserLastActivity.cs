// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// Holds information on when the user had a last activity
/// </summary>
/// <remarks>
/// The idea here is that the code online will do a read only from this
/// table. The table is maintained via database triggers.
/// </remarks>
[Table("user_last_activity")]
public class UserLastActivity
{
    /// <summary>
    /// The date / time of the activity
    /// </summary>
    public DateTime ActivityDate { get; set; }

    /// <summary>
    /// The id of the user
    /// </summary>
    public int UserId { get; set; }
}