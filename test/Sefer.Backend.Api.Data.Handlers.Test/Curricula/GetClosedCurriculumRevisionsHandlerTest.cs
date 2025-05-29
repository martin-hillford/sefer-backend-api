namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetClosedCurriculumRevisionsTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await PublishCurriculumRevisionHandlerTest.Initialize(context);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var revision = context.CurriculumRevisions.Single();
        revision.Stage = Stages.Closed;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));

        var revisions = await Handle(curriculum.Id);

        revisions.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle_NotClosed()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var revisions = await Handle(curriculum.Id);

        revisions.Count.Should().Be(0);
    }

    private async Task<List<CurriculumRevision>> Handle(int curriculumId)
    {
        var request = new GetClosedCurriculumRevisionsRequest(curriculumId);
        var provider = GetServiceProvider().Object;
        var handler = new GetClosedCurriculumRevisionsHandler(provider);
        return await handler.Handle(request, CancellationToken.None);
    }
}