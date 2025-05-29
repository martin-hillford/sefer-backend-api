namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetRewardByCodeKeyHandler(IServiceProvider serviceProvider)
    : Handler<GetRewardByCodeKeyRequest, Reward>(serviceProvider)
{
    public override async Task<Reward> Handle(GetRewardByCodeKeyRequest request, CancellationToken token)
    {
        var type = GetRewardType(request);
        if (type == null) return null;

        var context = GetDataContext();
        return await context.Rewards.FirstOrDefaultAsync(r => r.Type == type, token);
    }

    private RewardTypes? GetRewardType(GetRewardByCodeKeyRequest request)
    {
        return request.CodeKey switch
        {
            "voucher" => RewardTypes.VoucherReward,
            "curriculum" => RewardTypes.Curriculum,
            _ => null
        };
    }
}