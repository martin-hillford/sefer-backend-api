namespace Sefer.Backend.Api.Data.Algorithms;

/// <summary>
/// The curriculum processor is capable of processing if the user is working on a curriculum
/// </summary>
public class CurriculumRewardProcessor : AbstractRewardProcessor, IRewardProcessor
{
    /// <summary>
    /// Creates a new CurriculumRewardProcessor
    /// </summary>
    private CurriculumRewardProcessor(IMediator mediator, Reward reward) : base(mediator, reward) { }

    /// <summary>
    /// Determines of the enrollment if a reward will be awarded
    /// </summary>
    /// <returns>an empty list if no reward is granted, else a list of granted rewards</returns>
    public async Task<List<RewardGrant>> Process(Enrollment enrollment)
    {
        // 1) Get the targets the user still should reach (they refer use the target property to a curriculum)
        var targets = await GetNextTargets(enrollment.StudentId);
        if (targets.Any() == false) return new();

        // 2) Insert the enrollment
        if (await InsertEnrollment(enrollment) == false) return new();

        // 3) The next thing is to determine if by insert this enrollment the student has completed a curriculum
        var completed = await GetCompletedCurricula(enrollment);
        if (!completed.Any()) return new();

        // 4) determine which targets are reached
        var reached = targets.Where(t => completed.Any(c => c.Id == t.Target)).ToList();
        if (!reached.Any()) return new();

        // 5) Now reward the student

        // Define a storage for the received grants
        var received = new List<RewardGrant>();
        foreach (var target in reached)
        {
            // the target id is the id of the curriculum, we need the id of the revision here
            var revision = await Mediator.Send(new GetPublishedCurriculumRevisionRequest(target.Target));
            if (revision == null) continue;

            var grant = new RewardGrant
            {
                UserId = enrollment.StudentId,
                Date = DateTime.UtcNow,
                RewardId = Reward.Id,
                TargetId = target.Id,
                TargetReached = target.Target,
                TargetValue = revision.Id,
                Code = RewardProcessorFactory.GetRandomString(4) + "-" + RewardProcessorFactory.GetRandomString(2)
            };

            await Mediator.Send(new AddRewardGrantRequest(grant));
            grant.Reward = Reward;
        }

        // 6) return the result
        return received;
    }

    /// <summary>
    /// This method returns the next target for the user given the reward
    /// </summary>
    public async Task<List<RewardTarget>> GetNextTargets(int studentId)
    {
        return await Mediator.Send(new GetCurriculumRewardsNextTargetsRequest(studentId, Reward.Id));
    }

    /// <summary>
    /// This method will return all the curricula that are completed by this enrollment
    /// </summary>
    private async Task<List<Curriculum>> GetCompletedCurricula(Enrollment enrollment)
    {
        // Get all the curricula and the already (before this enrollment) completed courses by the student
        var curricula = await Mediator.Send(new GetPublishedCurriculaRequest(true));
        var studentCourses = await GetCompletedCourses(enrollment);
        var courseRevision = await Mediator.Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));

        // The trick is now to detect if the completed courseId would complete the curriculum
        var completed = new List<Curriculum>();
        foreach (var curriculum in curricula)
        {
            var courses = curriculum.GetPublishedCourses().Select(c => c.Id);
            var missing = courses.Except(studentCourses).ToList();
            if (missing.Count == 1 && missing.First() == courseRevision.CourseId) completed.Add(curriculum);
        }
        return completed;
    }

    /// <summary>
    /// Returns a list of ids of course completed by the student, except for the given enrollment
    /// </summary>
    private async Task<List<int>> GetCompletedCourses(Enrollment enrollment)
    {
        var enrollments = await Mediator.Send(new GetTakenCoursesOfStudentRequest(enrollment.StudentId));
        return enrollments
            .Where(e => e.Id != enrollment.Id && e is { ClosureDate: not null, IsCourseCompleted: true })
            .Select(e => e.CourseRevision.CourseId).Distinct().ToList();
    }

    /// <summary>
    /// This method will insert the enrollment and returns if the enrollment result in a new awarded course
    /// </summary>
    private async Task<bool> InsertEnrollment(Enrollment enrollment)
    {
        // 1) Check if the course of enrollment is in a curriculum
        var courseRevision = await Mediator.Send(new GetCourseRevisionByIdRequest(enrollment.CourseRevisionId));
        if (courseRevision == null) return false;

        var inCurriculum = await Mediator.Send(new IsCourseInPublishedCurriculumRequest(courseRevision.CourseId));
        if (!inCurriculum) return false;

        // 2) Oke, let's see if this enrollment is already being rewarded (because of retakes)
        var request = new IsEnrollmentAlreadyRewardedRequest(enrollment.StudentId, courseRevision.CourseId, Reward.Id);
        if (await Mediator.Send(request)) return false;

        // 3) Enrollment accepted
        var reward = new RewardEnrollment { EnrollmentId = enrollment.Id, RewardId = Reward.Id };
        return await Mediator.Send(new AddRewardEnrollmentRequest(reward));
    }
    
    public static CurriculumRewardProcessor Create(IMediator mediator, Reward reward)
        => new CurriculumRewardProcessor(mediator, reward);
    
}