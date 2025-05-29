namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// The voucher processor is capable of processing if the user will get a reward
/// </summary>
public abstract class AbstractRewardProcessor
{
    /// <summary>
    /// The dataContext to get the data from
    /// </summary>
    protected readonly IMediator Mediator;

    /// <summary>
    /// The reward that is used
    /// </summary>
    protected readonly Reward Reward;

    /// <summary>
    /// Creates an abstract RewardProcessor
    /// </summary>
    protected AbstractRewardProcessor(IMediator mediator, Reward reward)
    {
        Mediator = mediator;
        Reward = reward;
    }
}