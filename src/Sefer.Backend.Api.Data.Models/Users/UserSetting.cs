// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// Various applications may have additional settings for users.
/// To facilitate flexibility this class provides key value store for those settings.
/// These settings are for the front-end. The settings are stored here, the backend
/// has no responsibility in managing them.
/// </summary>
public class UserSetting : Entity
{
    /// <summary>
    /// The id of user which this setting applies to
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// The name of key of setting
    /// </summary>
    [Required, MaxLength(255)]
    public string KeyName { get; set; }
    
    /// <summary>
    /// The value of the settings
    /// </summary>
    [MaxLength(512)]
    public string Value { get; set; }
    
    /// <summary>
    /// The user which this setting applies to
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; }
}