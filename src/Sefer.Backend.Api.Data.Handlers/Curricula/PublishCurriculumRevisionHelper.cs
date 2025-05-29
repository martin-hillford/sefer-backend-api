namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class PublishCurriculumRevisionHelper : HelperClass
{
    internal PublishCurriculumRevisionHelper(IServiceProvider serviceProvider, DataContext context)
        : base(serviceProvider, context) { }

    internal bool Handle(int curriculumRevisionId)
    {
        var revision = Context.CurriculumRevisions.SingleOrDefault(r => r.Id == curriculumRevisionId);
        if (revision == null) return false;

        var curriculum = Context.CurriculumRevisions.SingleOrDefault(c => c.Id == revision.CurriculumId);
        if (curriculum == null) return false;

        var helper = new IsCurriculumRevisionPublishableHelper(ServiceProvider, Context);
        if (!helper.IsPublishable(revision)) return false;

        // Ensure to load the current published revision of the course that should be closed
        Context.Entry(revision).Reference(r => r.Curriculum).Load();
        var publishedRevisionHelper = new GetPublishedCurriculumRevisionHelper(Context);
        var previousPublishedRevision = publishedRevisionHelper.Get(revision.CurriculumId);

        // The first step is to create a new revision
        var newRevision = revision.CreateSuccessor();
        if (!Context.Insert(ServiceProvider, newRevision)) return false;

        // Next create a successor for each block
        var blocksHelper = new GetCurriculumBlocksHelper(Context);
        var blocks = blocksHelper.Handle(revision);
        foreach (var block in blocks)
        {
            // Next create a new block
            var success = CreateSuccessor(newRevision, block);
            if (success == false) return false;
        }

        // Everything is created now, only update the current revision
        revision.ModificationDate = DateTime.UtcNow;
        revision.Stage = Stages.Published;
        if (!Context.Update(ServiceProvider, revision)) return false;

        // Deal with any previous revision
        if (previousPublishedRevision != null)
        {
            var closeHelper = new CloseCurriculumRevisionHelper(ServiceProvider, Context);
            var closed = closeHelper.Close(previousPublishedRevision);
            if (closed == false) return false;
        }

        // Ensure that a reward for this curriculum exists
        EnsureCurriculumReward(revision.CurriculumId);

        // Everything done
        return true;

    }

    private bool CreateSuccessor(CurriculumRevision newRevision, CurriculumBlock block)
    {
        // First check if the revision and the lessons are valid (technically, if these fail there is a coding error)
        if (newRevision == null || !BaseValidation.IsValidEntity(ServiceProvider, newRevision)) return false;
        if (newRevision.Stage != Stages.Edit && newRevision.Stage != Stages.Test) return false;
        if (block == null || !BaseValidation.IsValidEntity(ServiceProvider, block)) return false;
        if (block.CurriculumRevisionId != newRevision.PredecessorId) return false;

        var newBlock = block.CreateSuccessor(newRevision);
        if (Context.Insert(ServiceProvider, newBlock) == false) return false;

        // Now create a copy of all the curriculum block courses
        var blocks = Context.CurriculumBlockCourses
            .Where(b => b.BlockId == block.Id)
            .Select(c => new CurriculumBlockCourse
            { BlockId = newBlock.Id, CourseId = c.CourseId, SequenceId = c.SequenceId })
            .ToList();

        foreach (var newCourseBlock in blocks)
        {
            if (Context.Insert(ServiceProvider, newCourseBlock) == false) return false;
        }

        return true;
    }

    private void EnsureCurriculumReward(int curriculumId)
    {
        var reward = Context.Rewards.FirstOrDefault(r => r.Type == RewardTypes.Curriculum);
        if (reward == null) return;

        var targets = Context.RewardTargets.Where(t => t.RewardId == reward.Id && t.Target == curriculumId);
        if (targets.Any()) return;

        Context.Insert(ServiceProvider, new RewardTarget { RewardId = reward.Id, Target = curriculumId, Value = 1d });
    }
}