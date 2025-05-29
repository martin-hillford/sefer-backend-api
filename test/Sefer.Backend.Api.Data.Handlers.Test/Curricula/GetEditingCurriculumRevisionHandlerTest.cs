namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetEditingCurriculumRevisionHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_NoCurriculum()
    {
        var retrieved = await Handle(-1);
        retrieved.Should().BeNull();
    }

    [TestMethod]
    [DataRow(Stages.Edit)]
    [DataRow(Stages.Test)]
    [DataRow(Stages.Published)]
    [DataRow(Stages.Closed)]
    public async Task Handle(Stages stage)
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        revision.Stage = stage;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));
        var retrieved = await Handle(revision.CurriculumId);
        (retrieved != null).Should().Be(stage is Stages.Edit or Stages.Test);
    }

    private async Task<CurriculumRevision?> Handle(int curriculumId)
    {
        var request = new GetEditingCurriculumRevisionRequest(curriculumId);
        var handler = new GetEditingCurriculumRevisionHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}