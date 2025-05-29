namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class CloseCurriculumRevisionHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_RevisionNotFound()
    {
        await Handle(19, false);
    }

    [TestMethod]
    [DataRow(Stages.Test)]
    [DataRow(Stages.Closed)]
    [DataRow(Stages.Edit)]
    public async Task Handle_NonPublishedRevision(Stages stage)
    {
        var (_, revision) = await PrepareRevision(stage);
        await Handle(revision.Id, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var (_, revision) = await PrepareRevision();

        await Handle(revision.Id, true);

        var context = GetDataContext();
        context.CurriculumRevisions.First().Stage.Should().Be(Stages.Closed);
    }

    [TestMethod]
    public async Task Handle_WithReward_NoTarget()
    {
        var (_, revision) = await PrepareRevision();

        var reward = new Reward { Type = RewardTypes.Curriculum, Description = "test", Name = "test" };
        await InsertAsync(reward);

        await Handle(revision.Id, true);
    }

    [TestMethod]
    public async Task Handle_WithReward()
    {
        var (curriculum, revision) = await PrepareRevision();

        var reward = new Reward { Type = RewardTypes.Curriculum, Description = "test", Name = "test" };
        await InsertAsync(reward);
        var target = new RewardTarget { RewardId = reward.Id, Target = curriculum.Id, IsDeleted = false };
        await InsertAsync(target);

        await Handle(revision.Id, true);

        var context = GetDataContext();
        context.RewardTargets.First().IsDeleted.Should().BeTrue();
    }

    private async Task<(Curriculum, CurriculumRevision)> PrepareRevision(Stages stage = Stages.Published)
    {
        var curriculum = new Curriculum { Name = "Name", Description = "Description " };
        await InsertAsync(curriculum);
        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = stage };
        await InsertAsync(revision);
        return (curriculum, revision);
    }

    private async Task Handle(int revisionId, bool expected)
    {
        var request = new CloseCurriculumRevisionRequest(revisionId);
        var handler = new CloseCurriculumRevisionHandler(GetServiceProvider().Object);

        var result = await handler.Handle(request, CancellationToken.None);

        result.Should().Be(expected);
    }
}