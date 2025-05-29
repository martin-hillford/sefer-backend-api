// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Rewards;

/// <summary>
/// A reward is just an indicator assigned to users they completed certain courses
/// </summary>
/// <remarks>
/// In a later stage of the development the decision was made to make this configurable.
/// So this now filled by the programmer
/// </remarks>
public class Reward : Entity
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    [Required, MaxLength(255), MinLength(3)]
    public string Name { get; set; }

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    [Required, MaxLength(int.MaxValue)]
    public string Description { get; set; }

    /// <summary>
    /// Defines the minimal required grade for this reward
    /// </summary>
    public double? MinimalGrade { get; set; }

    /// <summary>
    /// The type of reward
    /// </summary>
    /// <value></value>
    public RewardMethods Method { get; set; }

    /// <summary>
    /// Holds the type of the reward
    /// </summary>
    /// <value></value>
    public RewardTypes Type { get; set; } = RewardTypes.NotSet;

    #endregion

    #region Collections

    /// <summary>
    /// The targets for this reward
    /// </summary>
    public ICollection<RewardTarget> Targets { get; set; }

    #endregion
}

/// <summary>
/// The type of reward
/// </summary>
public enum RewardMethods : short { Staffeled = 1, Recurring = 2, Single = 3 }

/// <summary>
/// The method of reward
/// </summary>
public enum RewardTypes : short { NotSet = 0, VoucherReward = 1, Curriculum = 2 }