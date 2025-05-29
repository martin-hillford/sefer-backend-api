namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class IsCurriculumBlockNameUniqueHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_NameEmpty()
    {
        await Handle_NameEmpty(null);
        await Handle_NameEmpty(string.Empty);
        await Handle_NameEmpty("     ");
    }

    [TestMethod]
    public async Task Handle_DifferentYear()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(block.Id, curriculum.Id, block.Year + 1, block.Name);
        await Handle(request, true);
    }

    [TestMethod]
    public async Task Handle_DifferentCurriculum()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(block.Id, curriculum.Id + 1, block.Year, block.Name);
        await Handle(request, true);
    }

    [TestMethod]
    public async Task Handle_DifferentBlock()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(block.Id + 1, curriculum.Id, block.Year, block.Name);
        await Handle(request, false);
    }

    [TestMethod]
    public async Task Handle_SameBlock()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(block.Id, curriculum.Id, block.Year, block.Name);
        await Handle(request, true);
    }

    [TestMethod]
    [DataRow("block", false)]
    [DataRow("block ", false)]
    [DataRow("Block", false)]
    [DataRow("Block 2", true)]
    public async Task Handle(string requestName, bool expected)
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(block.Id + 1, curriculum.Id, block.Year, requestName);
        await Handle(request, expected);
    }

    private async Task Handle_NameEmpty(string? value)
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var request = new IsCurriculumBlockNameUniqueRequest(13, curriculum.Id, 0, value);
        await Handle(request, false);
    }

    private async Task Handle(IsCurriculumBlockNameUniqueRequest request, bool expected)
    {
        var handler = new IsCurriculumBlockNameUniqueHandler(GetServiceProvider().Object);
        var isUnique = await handler.Handle(request, CancellationToken.None);
        isUnique.Should().Be(expected);
    }
}