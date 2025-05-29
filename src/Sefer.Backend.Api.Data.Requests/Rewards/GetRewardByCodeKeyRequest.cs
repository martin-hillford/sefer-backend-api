namespace Sefer.Backend.Api.Data.Requests.Rewards;

public class GetRewardByCodeKeyRequest(string codeKey) : IRequest<Reward>
{
    public readonly string CodeKey = codeKey;
}