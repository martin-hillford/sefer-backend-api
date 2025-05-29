namespace Sefer.Backend.Api.Data.Handlers.Curricula;

internal class CloseCurriculumRevisionHelper : HelperClass
{
    internal CloseCurriculumRevisionHelper(IServiceProvider serviceProvider, DataContext context)
        : base(serviceProvider, context) { }

    internal bool Close(CurriculumRevision revision)
    {
        // check if we have a revision that can be closed
        if (revision is not { Stage: Stages.Published }) return false;

        // Everything is created now, only update the current revision
        revision.ModificationDate = DateTime.UtcNow;
        revision.Stage = Stages.Closed;

        var updated = Context.Update(ServiceProvider, revision);
        if (!updated) return false;

        // Deal with the rewarding system
        CheckRewardTargetRemoval(revision.CurriculumId);
        return true;
    }

    private void CheckRewardTargetRemoval(int curriculumId)
    {
        var reward = Context.Rewards.FirstOrDefault(r => r.Type == RewardTypes.Curriculum);
        if (reward == null) return;

        var helper = new GetPublishedCurriculumRevisionHelper(Context);
        var publishedRevision = helper.Get(curriculumId);
        if (publishedRevision != null) return;

        var targets = Context.RewardTargets.Where(t => t.RewardId == reward.Id && t.Target == curriculumId).ToList();
        if (targets.Any() == false) return;

        foreach (var target in targets) { target.IsDeleted = true; }
        Context.SaveChanges();
    }
}
