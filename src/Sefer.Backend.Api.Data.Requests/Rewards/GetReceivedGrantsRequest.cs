namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetReceivedGrantsRequest(int studentId) : IRequest<List<RewardGrant>>
{
    public readonly int StudentId = studentId;
}