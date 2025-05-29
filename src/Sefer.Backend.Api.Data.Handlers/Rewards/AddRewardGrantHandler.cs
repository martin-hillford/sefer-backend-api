namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class AddRewardGrantHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddRewardGrantRequest, RewardGrant>(serviceProvider);