namespace Sefer.Backend.Api.Data.Handlers.Rewards;

public class GetRewardedEnrollmentsHandler(IServiceProvider serviceProvider)
    : Handler<GetRewardedEnrollmentsRequest, List<RewardEnrollment>>(serviceProvider)
{
    public override async Task<List<RewardEnrollment>> Handle(GetRewardedEnrollmentsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var student = await context.Users.SingleOrDefaultAsync(u => u.Id == request.StudentId, token);
        var reward = await context.Rewards.SingleOrDefaultAsync(r => r.Id == request.RewardId, token);
        if (student == null || reward == null) return new List<RewardEnrollment>();

        // query for enrollments that are award, but don't have a grant yet
        var enrollments = await context.RewardEnrollments
            .Include(r => r.Enrollment)
            .ThenInclude(e => e.CourseRevision)
            .ThenInclude(r => r.Course)
            .Where(r => r.RewardId == reward.Id && r.Enrollment.StudentId == student.Id)
            .ToListAsync(cancellationToken: token);
        return enrollments;
    }
}