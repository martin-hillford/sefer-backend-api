namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class IsEnrollmentAlreadyRewardedRequest(int studentId, int courseId, int rewardId) : IRequest<bool>
{
    public readonly int StudentId = studentId;

    public readonly int CourseId = courseId;

    public readonly int RewardId = rewardId;
}