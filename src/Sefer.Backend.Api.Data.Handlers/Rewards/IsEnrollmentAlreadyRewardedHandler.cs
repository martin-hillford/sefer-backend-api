namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class IsEnrollmentAlreadyRewardedHandler(IServiceProvider serviceProvider)
    : Handler<IsEnrollmentAlreadyRewardedRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsEnrollmentAlreadyRewardedRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.RewardEnrollments
            .AnyAsync(r => r.Enrollment.StudentId == request.StudentId && r.Enrollment.CourseRevision.CourseId == request.CourseId && r.RewardId == request.RewardId, cancellationToken: token);
    }
}