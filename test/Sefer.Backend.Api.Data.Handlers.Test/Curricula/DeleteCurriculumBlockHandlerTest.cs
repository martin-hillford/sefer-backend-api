namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class DeleteCurriculumBlockHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_BlockIsNull()
    {
        var block = new CurriculumBlock();
        await Handle(block, false);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        await CurriculumUnitTest.InitializeCurriculumBlockCourse(context);
        var block = await context.CurriculumBlocks.SingleAsync();

        await Handle(block, true);
    }

    private async Task Handle(CurriculumBlock block, bool expected, MockedServiceProvider? provider = null)
    {
        var request = new DeleteCurriculumBlockRequest(block);
        var handler = new DeleteCurriculumBlockHandler((provider ?? GetServiceProvider()).Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Should().Be(expected);
    }
}