namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class SubmitForGrantRequest(int enrollmentId) : IRequest<List<RewardGrant>>
{
    public readonly int EnrollmentId = enrollmentId;
}