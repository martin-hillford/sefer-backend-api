namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class AddCurriculumBlockHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced };
        await InsertAsync(curriculum);

        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
        await InsertAsync(revision);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();

        var block = new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };

        var request = new AddCurriculumBlockRequest(block);
        var handler = new AddCurriculumBlockHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsTrue(result);

        await using var data = GetContext();
        Assert.AreEqual(1, data.CurriculumBlocks.Count());
    }
}