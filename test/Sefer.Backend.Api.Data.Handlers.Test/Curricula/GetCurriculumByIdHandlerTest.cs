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
        curriculum.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle_WithoutRevisions()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id, false);
        retrieved.Should().NotBeNull();
        retrieved.Revisions.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle_WithRevision()
    {
        var context = GetDataContext();
        var curriculum = context.Curricula.Single();
        var retrieved = await Handle(curriculum.Id, true);
        retrieved.Should().NotBeNull();
        retrieved.Revisions.Count.Should().Be(1);
    }

    private async Task<Curriculum> Handle(int curriculumId, bool includeRevisions)
    {
        var request = new GetCurriculumByIdRequest(curriculumId, includeRevisions);
        var handler = new GetCurriculumByIdHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}