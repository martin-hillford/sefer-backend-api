using Microsoft.Extensions.Configuration;

namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetNextTargetsHandler(IServiceProvider serviceProvider)
    : Handler<GetNextTargetsRequest, List<RewardTarget>>(serviceProvider)
{
    public override async Task<List<RewardTarget>> Handle(GetNextTargetsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var config = ServiceProvider.GetService<IConfiguration>();
        
        var student = await context.Users.SingleOrDefaultAsync(u => u.Id == request.UserId, token);
        var reward = await context.Rewards.SingleOrDefaultAsync(r => r.Id == request.RewardId, token);
        if (reward == null || student == null) return [];
        
        var processor = RewardProcessorFactory.GetProcessor(Mediator, reward, config);
        if (processor == null) return [];
        
        return await processor.GetNextTargets(student.Id);
    }
}