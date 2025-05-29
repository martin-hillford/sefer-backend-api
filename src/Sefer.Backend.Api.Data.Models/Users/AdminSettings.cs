namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// The settings for the
/// </summary>
/// <inheritdoc />
public class AdminSettings : Entity
{
    #region Properties

    /// <summary>
    /// The id of the admin these settings do apply for
    /// </summary>
    public int AdminId { get; set; }

    /// <summary>
    /// Holds if this a public admin. A public admin will be available to answer questions of users
    /// </summary>
    public bool IsPublicAdmin { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The mentor of the setting
    /// </summary>
    [ForeignKey(nameof(AdminId))]
    public User Admin { get; set; }

    #endregion
}
