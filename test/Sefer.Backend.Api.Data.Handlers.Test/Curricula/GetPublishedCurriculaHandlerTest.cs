namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetPublishedCurriculaHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await PublishCurriculumRevisionHandlerTest.Initialize(context);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var revision = context.CurriculumRevisions.Single();
        revision.Stage = Stages.Published;
        context.UpdateSingleProperty(revision, nameof(revision.Stage));

        var curricula = await Handle(true);

        curricula.Count.Should().Be(1);
        curricula[0].PublishedCurriculumRevision.Should().NotBeNull();
    }

    [TestMethod]
    public async Task Handle_NotPublished()
    {
        var curricula = await Handle(true);
        curricula.Count.Should().Be(0);
    }

    private async Task<List<Curriculum>> Handle(bool includeCourses)
    {
        var request = new GetPublishedCurriculaRequest(includeCourses);
        var provider = GetServiceProvider().Object;
        var handler = new GetPublishedCurriculaHandler(provider);
        return await handler.Handle(request, CancellationToken.None);
    }
}