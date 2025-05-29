// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Rewards;

/// <summary>
/// Rewards can be staffeled, so every reward can have own or more targets
/// </summary>
public class RewardTarget : Entity
{
    /// <summary>
    /// The identifier of the reward this target applies to
    /// </summary>
    public int RewardId { get; set; }

    /// <summary>
    /// The target
    /// </summary>
    public int Target { get; set; }

    /// <summary>
    /// The value of the target
    /// </summary>
    /// <remarks>Ensure that when having staffeled rewards, the value is cumulative!</remarks>
    public double? Value { get; set; }

    /// <summary>
    /// Holds the order in which the targets should be met (combination reward id en target should be unique)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Holds if this target was deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// A reference to the reward of this target
    /// </summary>
    [ForeignKey("RewardId")]
    public Reward Reward { get; set; }

    /// <summary>
    /// A target can be met by many grants
    /// </summary>
    [InverseProperty(nameof(RewardGrant.Target))]
    public ICollection<RewardGrant> Grants { get; set; }
}