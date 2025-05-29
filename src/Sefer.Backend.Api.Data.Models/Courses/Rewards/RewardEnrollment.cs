// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Rewards;

/// <summary>
/// This object contains the enrollments that have qualified for a reward
/// </summary>
public class RewardEnrollment : Entity
{
    /// <summary>
    /// The enrollment this reward is about
    /// </summary>
    public int EnrollmentId { get; set; }

    /// <summary>
    /// The id of the reward
    /// </summary>
    public int RewardId { get; set; }

    /// <summary>
    /// The enrollment this reward is about
    /// </summary>
    [ForeignKey("EnrollmentId")]
    public Enrollment Enrollment { get; set; }

    /// <summary>
    /// The reward
    /// </summary>
    [ForeignKey("RewardId")]
    public Reward Reward { get; set; }
}