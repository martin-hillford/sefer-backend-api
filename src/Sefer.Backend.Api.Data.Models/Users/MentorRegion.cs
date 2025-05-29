namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// Defines to which regions (nl, be, etc.) this mentor belongs
/// </summary>
public class MentorRegion : Entity
{
    /// <summary>
    /// The id of the mentor is that mentoring
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The region the mentor is teaching in
    /// </summary>
    [MaxLength(50)]
    public string RegionId { get; set; }

    /// <summary>
    /// The mentor is that is mentoring
    /// </summary>
    [ForeignKey("MentorId")]
    public User Mentor { get; set; }
}