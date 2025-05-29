namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCoursesByCurriculumRevisionHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlockCourse();

    [TestMethod]
    public async Task Handle_NotFound()
    {
        var courses = await Handle(-1);
        courses.Any().Should().BeFalse();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        var courses = await Handle(revision.Id);
        courses.Count.Should().Be(1);
    }

    private async Task<List<Course>> Handle(int revisionId)
    {
        var request = new GetCoursesByCurriculumRevisionRequest(revisionId);
        var handler = new GetCoursesByCurriculumRevisionHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}