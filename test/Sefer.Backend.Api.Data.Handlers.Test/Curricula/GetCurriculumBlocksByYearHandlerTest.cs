namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumBlocksByYearHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_RevisionNull() => await Handle(-1, 0, 0);

    [TestMethod]
    public async Task Handle_SingleBlock()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        var block = context.CurriculumBlocks.Single();
        await Handle(revision.Id, block.Year, 1);
    }

    [TestMethod]
    public async Task Handle_MultipleBlocks()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        var block = context.CurriculumBlocks.Single();

        await CreateNewBlock(block.Id, block.SequenceId + 3);
        await CreateNewBlock(block.Id, block.SequenceId + 1);
        await CreateNewBlock(block.Id, block.SequenceId + 2);

        await Handle(revision.Id, block.Year, 4);
    }

    private async Task CreateNewBlock(int baseBlockId, int sequence)
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        var block = context.CurriculumBlocks.Single(b => b.Id == baseBlockId);

        var created = new CurriculumBlock
        {
            Year = block.Year,
            SequenceId = block.SequenceId + sequence,
            Name = $"{block.Name}_{sequence}",
            Description = $"{block.Description}_{sequence}",
            CurriculumRevisionId = revision.Id
        };
        await InsertAsync(created);
    }

    private async Task Handle(int revisionId, short year, int expectedCount)
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.SingleOrDefault(c => c.Id == revisionId);

        var provider = GetServiceProvider();
        if(revision != null) provider.AddRequestResults<GetCurriculumRevisionByIdRequest, CurriculumRevision>(revision);

        var request = new GetCurriculumBlocksByYearRequest(revisionId, year);
        var handler = new GetCurriculumBlocksByYearHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        result.Count.Should().Be(expectedCount);

        if (result.Count == 0) return;

        var sequence = result.Select(b => b.SequenceId).ToList();
        var expected = result.Select(b => b.SequenceId).OrderBy(b => b).ToList();

        for (var index = 0; index < sequence.Count; index++)
        {
            sequence[index].Should().Be(expected[index]);
        }
    }
}