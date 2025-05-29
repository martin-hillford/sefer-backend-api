namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetGrantByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetGrantByIdRequest, RewardGrant>(serviceProvider)
{
    public override async Task<RewardGrant> Handle(GetGrantByIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.RewardGrants
            .Where(g => g.Id == request.GrantId)
            .Include(g => g.Target)
            .FirstOrDefaultAsync(token);
    }
}