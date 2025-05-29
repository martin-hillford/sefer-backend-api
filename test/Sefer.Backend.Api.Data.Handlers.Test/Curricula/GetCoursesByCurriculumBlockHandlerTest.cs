namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCoursesByCurriculumBlockHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlockCourse();

    [TestMethod]
    public async Task Handle_BlockNull()
    {
        var courses = await Handle(-1, false);
        courses.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task Handle_PublishedOnly()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var courses = await Handle(block.Id, true);
        courses.Count.Should().Be(0);
    }

    [TestMethod]
    public async Task Handle_WithPublished()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var revision = context.CourseRevisions.Single();
        revision.Stage = Stages.Published;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));

        var courses = await Handle(block.Id, true);
        courses.Count.Should().Be(1);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var block = context.CurriculumBlocks.Single();
        var courses = await Handle(block.Id, false);
        courses.Count.Should().Be(1);
    }

    private async Task<List<Course>> Handle(int blockId, bool publishedOnly)
    {
        var request = new GetCoursesByCurriculumBlockRequest(blockId, publishedOnly);
        var handler = new GetCoursesByCurriculumBlockHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}