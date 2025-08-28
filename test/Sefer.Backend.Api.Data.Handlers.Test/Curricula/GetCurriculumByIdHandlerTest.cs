namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumByIdHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle_NotFound()
    {
        var curriculum = await Handle(-1, true);
        Assert.IsNull(curriculum);
    }

    [TestMethod]
    public async Task Handle_WithoutRevisions()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id, false);
        Assert.IsNotNull(retrieved);
        Assert.IsNull(retrieved.Revisions);
    }

    [TestMethod]
    public async Task Handle_WithRevision()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id, true);
        Assert.IsNotNull(retrieved);
        Assert.AreEqual(1, retrieved.Revisions.Count);
    }

    private async Task<Curriculum> Handle(int curriculumId, bool includeRevisions)
    {
        var request = new GetCurriculumByIdRequest(curriculumId, includeRevisions);
        var handler = new GetCurriculumByIdHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}