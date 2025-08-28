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
        Assert.IsNull(retrieved);
    }

    [TestMethod]
    public async Task Handle_NotPublished()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id);
        Assert.IsNull(retrieved);
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
        Assert.IsNotNull(retrieved);
    }

    private async Task<CurriculumRevision> Handle(int curriculumId)
    {
        var request = new GetPublishedCurriculumRevisionRequest(curriculumId);
        var handler = new GetPublishedCurriculumRevisionHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}