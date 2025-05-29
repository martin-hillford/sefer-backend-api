namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetCurriculumRewardsNextTargetsHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumRewardsNextTargetsRequest, List<RewardTarget>>(serviceProvider)
{
    public override async Task<List<RewardTarget>> Handle(GetCurriculumRewardsNextTargetsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.RewardTargets
            .Where(t =>
                t.IsDeleted == false &&
                t.RewardId == request.RewardId &&
                !context.RewardGrants.Any(g => g.TargetId == t.Id && g.UserId == request.StudentId)
            )
            .ToListAsync(cancellationToken: token);
    }
}