// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.Models.Courses.Rewards;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// This is the view to be sent to the student with information on the reward system
/// </summary>
public class RewardView
{
    /// <summary>
    /// The current enrollments that are in count for the reward
    /// </summary>
    public readonly int Enrollments;

    /// <summary>
    /// The number of enrollments that must be reached before a reward is given
    /// </summary>
    public readonly int? RewardCount;

    /// <summary>
    /// The value of the reward (currently in euro)
    /// </summary>
    public readonly double? RewardMoney;

    /// <summary>
    /// Returns if the reward system is enabled at all
    /// </summary>
    public bool RewardsEnabled => RewardCount.HasValue && RewardMoney.HasValue;

    /// <summary>
    /// Creates a new reward view
    /// </summary>
    /// <param name="nextTarget">The next target for the student</param>
    /// <param name="enrollments">The current enrollments</param>
    public RewardView(RewardTarget nextTarget, int enrollments)
    {
        Enrollments = enrollments;
        RewardCount = nextTarget.Target;
        RewardMoney = nextTarget.Value;
    }
}