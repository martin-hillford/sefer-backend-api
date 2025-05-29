namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetTargetsByRewardHandler(IServiceProvider serviceProvider)
    : Handler<GetTargetsByRewardRequest, List<RewardTarget>>(serviceProvider)
{
    public override async Task<List<RewardTarget>> Handle(GetTargetsByRewardRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.RewardTargets
            .Where(t => t.IsDeleted == false && t.RewardId == request.RewardId)
            .ToListAsync(cancellationToken: token);
    }
}