namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// Defines an interface for reward processors
/// </summary>
public interface IRewardProcessor
{
    /// <summary>
    /// Determines of the enrollment if a reward will be awarded
    /// </summary>
    /// <returns>null if no reward is granted, else the granted reward</returns>
    Task<List<RewardGrant>> Process(Enrollment enrollment);

    /// <summary>
    /// This method returns the next target for the user given the reward
    /// </summary>
    Task<List<RewardTarget>> GetNextTargets(int studentId);
}