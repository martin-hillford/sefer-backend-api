using Microsoft.Extensions.Configuration;

namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// The reward process factory create reward processors
/// </summary>
public static class RewardProcessorFactory
{
    public static IRewardProcessor? GetProcessor(IMediator mediator, Reward? reward, IConfiguration config)
    {
        if (reward == null) return null;
        return reward.Type switch
        {
            RewardTypes.VoucherReward => VoucherRewardProcessor.Create(mediator, reward, config),
            RewardTypes.Curriculum => CurriculumRewardProcessor.Create(mediator, reward),
            _ => null
        };
    }

    public static async Task<IEnumerable<IRewardProcessor>> GetProcessors(IMediator? mediator, IConfiguration config)
    {
        if (mediator == null) return [];
        var rewards = await mediator.Send(new GetRewardsRequest());
        return rewards.Select(reward => GetProcessor(mediator, reward, config)).OfType<IRewardProcessor>();
    }

    /// <summary>
    /// Returns a random string of the given length. Can be used for verification codes of grants
    /// </summary>
    public static string GetRandomString(int length)
    {
        var provider = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        provider.GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "");
    }
}