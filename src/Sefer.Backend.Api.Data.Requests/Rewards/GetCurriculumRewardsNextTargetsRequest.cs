namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetCurriculumRewardsNextTargetsRequest(int studentId, int rewardId) : IRequest<List<RewardTarget>>
{
    public readonly int RewardId = rewardId;

    public readonly int StudentId = studentId;
}