namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumOverallStageHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_Published()
    {
        var revision = new CurriculumRevision { Stage = Stages.Published };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetPublishedCurriculumRevisionRequest, CurriculumRevision>(revision);
        await Handle(provider, Stages.Published);
    }
    
    [TestMethod]
    public async Task Handle_Closed()
    {
        var revisions = new List<CurriculumRevision> { new() { Stage = Stages.Closed } };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetClosedCurriculumRevisionsRequest, List<CurriculumRevision>>(revisions);
        await Handle(provider, Stages.Closed);
    }
    
    [TestMethod]
    public async Task Handle_Editing()
    {
        var revision = new CurriculumRevision { Stage = Stages.Edit };
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetClosedCurriculumRevisionsRequest, List<CurriculumRevision>>([]);
        provider.AddRequestResult<GetEditingCurriculumRevisionRequest, CurriculumRevision>(revision);
        await Handle(provider, Stages.Edit);
    }
    
    [TestMethod]
    public async Task Handle_Null()
    {
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetClosedCurriculumRevisionsRequest, List<CurriculumRevision>>([]);
        await Handle(provider, null);
    }

    private static async Task Handle(MockedServiceProvider provider, Stages? expected)
    {
        var request = new GetCurriculumOverallStageRequest(13);
        var handler = new GetCurriculumOverallStageHandler(provider.Object);
        var stage = await handler.Handle(request, CancellationToken.None);
        stage.Should().Be(expected);
    }
}