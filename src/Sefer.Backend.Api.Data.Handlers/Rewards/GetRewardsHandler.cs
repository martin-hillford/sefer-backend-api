namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetRewardsHandler(IServiceProvider serviceProvider)
    : Handler<GetRewardsRequest, List<Reward>>(serviceProvider)
{
    public override async Task<List<Reward>> Handle(GetRewardsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Rewards.ToListAsync(cancellationToken: token);
    }
}



