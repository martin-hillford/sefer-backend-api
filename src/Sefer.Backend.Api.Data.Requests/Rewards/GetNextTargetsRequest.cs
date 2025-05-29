namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetNextTargetsRequest(int userId, int rewardId) : IRequest<List<RewardTarget>>
{
    public readonly int RewardId = rewardId;

    public readonly int UserId = userId;
}