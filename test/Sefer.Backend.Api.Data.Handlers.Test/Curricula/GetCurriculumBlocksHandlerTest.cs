namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumBlocksHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_RevisionNull() => await Handle(-1, 0);

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        await Handle(revision.Id, 1);
    }

    private async Task Handle(int revisionId, int expectedCount)
    {
        var request = new GetCurriculumBlocksRequest(revisionId);
        var handler = new GetCurriculumBlocksHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Count.Should().Be(expectedCount);
    }
}