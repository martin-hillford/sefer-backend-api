using Microsoft.Extensions.Configuration;
using Sefer.Backend.SharedConfig.Lib;

namespace Sefer.Backend.Api.Data.Algorithms;

public class VoucherRewardProcessor : AbstractRewardProcessor, IRewardProcessor
{
    private VoucherRewardProcessor(IMediator mediator, Reward reward)
        : base(mediator, reward) { }
    
    public async Task<List<RewardGrant>> Process(Enrollment enrollment)
    {
        // 1) Validate and insert the enrollment.
        if (!await ValidateAndInsertEnrollment(enrollment)) return new();

        // 2) Now load enrollments that count toward this reward
        var enrollments = await Mediator.Send(new GetRewardedEnrollmentsRequest(enrollment.StudentId, Reward.Id));
        if (enrollments.Count == 0) return [];

        var target = await GetNextTarget(enrollment.StudentId);
        if (target == null || enrollments.Count != target.Target) return [];

        // 3) Cool the user receive a reward!
        var grant = new RewardGrant
        {
            UserId = enrollment.StudentId,
            Date = DateTime.UtcNow,
            RewardId = Reward.Id,
            TargetId = target.Id,
            TargetReached = target.Target,
            TargetValue = target.Value,
            Code = RewardProcessorFactory.GetRandomString(4) + "-" + RewardProcessorFactory.GetRandomString(2)
        };
        await Mediator.Send(new AddRewardGrantRequest(grant));

        grant.Reward = Reward;
        return [grant];
    }

    private async Task<bool> ValidateAndInsertEnrollment(Enrollment enrollment)
    {
        // 1) Check if the enrollment is self is valid enrollment to get a grant. Also check if the enrollment meets the grade requirements
        if (enrollment.IsCourseCompleted == false || enrollment.Imported) return false;
        if (Reward.MinimalGrade.HasValue && (enrollment.Grade.HasValue == false || enrollment.Grade < Reward.MinimalGrade)) return false;

        // 2) Get the course revision of the enrollment
        var courseRevision = await Mediator.Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return false;

        // 2) Oke, let's check if this happened to be a retake since retakes do not result in a new grant (because it is already awarded)
        var request = new IsEnrollmentAlreadyRewardedRequest(enrollment.StudentId, courseRevision.CourseId, Reward.Id);
        if (await Mediator.Send(request)) return false;

        // 3) Enrollment qualified
        var rewardEnrollment = new RewardEnrollment { EnrollmentId = enrollment.Id, RewardId = Reward.Id };
        return await Mediator.Send(new AddRewardEnrollmentRequest(rewardEnrollment));
    }

    private async Task<RewardTarget?> GetNextTarget(int studentId)
    {
        // Get a list of all the targets that the student has met for this reward
        var grants = await Mediator.Send(new GetReceivedGrantsRequest(studentId));
        var reached = grants.Where(g => g.RewardId == Reward.Id).Select(g => g.TargetId).ToList();

        // These are all the targets still to reach for the student
        var targets = await Mediator.Send(new GetTargetsByRewardRequest(Reward.Id));
        var unreached = targets.Where(t => t.IsDeleted == false && t.RewardId == Reward.Id && reached.Contains(t.Id) == false);

        // return the first target to reach
        return unreached.OrderBy(t => t.Order).FirstOrDefault();
    }

    public async Task<List<RewardTarget>> GetNextTargets(int studentId)
    {
        var target = await GetNextTarget(studentId);
        var result = new List<RewardTarget>();
        if (target != null) result.Add(target);
        return result;
    }

    public static VoucherRewardProcessor? Create(IMediator mediator, Reward reward, IConfiguration config)
    {
        var enabled =
            EnvVar.GetEnvironmentVariable("VOUCHER_REWARD_ENABLED") == "true" ||
            config.GetValue<bool>("VoucherRewardEnabled");
        return !enabled ? null : new VoucherRewardProcessor(mediator, reward);
    }
}