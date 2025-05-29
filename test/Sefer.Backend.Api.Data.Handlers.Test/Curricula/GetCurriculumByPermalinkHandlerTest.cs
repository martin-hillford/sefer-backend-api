namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumByPermalinkHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_PermalinkEmpty()
    {
        await Handle(null);
        await Handle(string.Empty);
        await Handle("     ");
    }

    [TestMethod]
    public async Task Handle_NoPermalink()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle("permalink", curriculum);
    }

    [TestMethod]
    [DataRow("block")]
    [DataRow("block ")]
    [DataRow("Block")]
    public async Task Handle(string? permalink)
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        await Handle(permalink, curriculum);
    }

    private async Task Handle(string? permalink, IEntity? curriculum)
    {
        var request = new GetCurriculumByPermalinkRequest(permalink);
        var handler = new GetCurriculumByPermalinkHandler(GetServiceProvider().Object);
        var retrieved = await handler.Handle(request, CancellationToken.None);
        retrieved?.Id.Should().Be(curriculum?.Id);
    }
}