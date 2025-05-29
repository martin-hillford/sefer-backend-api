
namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class AddRewardEnrollmentHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddRewardEnrollmentRequest, RewardEnrollment>(serviceProvider);



