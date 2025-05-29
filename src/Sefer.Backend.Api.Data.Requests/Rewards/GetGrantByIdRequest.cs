namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetGrantByIdRequest(int grantId) : IRequest<RewardGrant>
{
    public readonly int GrantId = grantId;
}