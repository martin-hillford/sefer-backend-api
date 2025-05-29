namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetRewardedEnrollmentsRequest(int studentId, int rewardId) : IRequest<List<RewardEnrollment>>
{
    public readonly int StudentId = studentId;

    public readonly int RewardId = rewardId;
}