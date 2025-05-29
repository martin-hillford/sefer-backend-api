namespace Sefer.Backend.Api.Data.Models.Courses.Rewards;

/// <summary>
/// A reward grant is a model that defines that a reward is award on a certain day
/// </summary>
public class RewardGrant : Entity
{
    /// <summary>
    /// The date the grant is reward
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// The id of the user (student) to which this reward is grant to
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// should contain the target that was reached
    /// </summary>
    public int TargetReached { get; set; }

    /// <summary>
    /// should contain the target value at the time
    /// </summary>
    public double? TargetValue { get; set; }

    /// <summary>
    /// The id of the target that has been met
    /// </summary>
    public int TargetId { get; set; }

    /// <summary>
    /// The id of reward this reward is about
    /// </summary>
    public int RewardId { get; set; }

    /// <summary>
    /// A code can be assigned at creation of the object for verification purposes
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Code { get; set; }

    /// <summary>
    /// The user (student) to which this reward is grant to
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; set; }

    /// <summary>
    /// The target that has been met
    /// </summary>
    [ForeignKey("TargetId")]
    public RewardTarget Target { get; set; }

    /// <summary>
    /// The reward is grant is about
    /// </summary>
    [ForeignKey("RewardId")]
    public Reward Reward { get; set; }

    /// <summary>
    /// The enrollments for this reward
    /// </summary>
    /// <remarks>Given the null value it is a bit annoying in ef core, load them manually</remarks>
    [NotMapped]
    public List<RewardEnrollment> Enrollments { get; set; }
}