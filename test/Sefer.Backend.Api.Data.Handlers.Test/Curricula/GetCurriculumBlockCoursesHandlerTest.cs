namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumBlockCoursesHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlockCourse();

    [TestMethod]
    public async Task Handle_BlockNull() => await Handle(-1, 0);

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        await Handle(block.Id, 1);
    }

    private async Task Handle(int revisionId, int expectedCount)
    {
        var request = new GetCurriculumBlockCoursesRequest(revisionId);
        var handler = new GetCurriculumBlockCoursesHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Count.Should().Be(expectedCount);
    }
}