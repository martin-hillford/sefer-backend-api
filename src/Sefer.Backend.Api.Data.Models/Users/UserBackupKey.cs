using System.Diagnostics.CodeAnalysis;

namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// The settings for the
/// </summary>
/// <inheritdoc />
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class UserBackupKey : Entity
{
    #region Properties

    /// <summary>
    /// The id of the user this key is a backup for
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The (encrypted) backup key
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string BackupKey { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The user to which the backup key belongs
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; }

    #endregion
}