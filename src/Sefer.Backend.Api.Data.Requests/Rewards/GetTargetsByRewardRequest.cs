namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetTargetsByRewardRequest(int rewardId) : IRequest<List<RewardTarget>>
{
    public readonly int RewardId = rewardId;
}