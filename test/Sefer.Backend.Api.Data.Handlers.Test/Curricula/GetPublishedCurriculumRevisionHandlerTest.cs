namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetPublishedCurriculumRevisionHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_CurriculumNull()
    {
        var retrieved = await Handle(-1);
        retrieved.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle_NotPublished()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id);
        retrieved.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var revision = context.CurriculumRevisions.Single();
        revision.Stage = Stages.Published;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));
        var retrieved = await Handle(curriculum.Id);
        retrieved.Should().NotBeNull();
    }

    private async Task<CurriculumRevision> Handle(int curriculumId)
    {
        var request = new GetPublishedCurriculumRevisionRequest(curriculumId);
        var handler = new GetPublishedCurriculumRevisionHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}