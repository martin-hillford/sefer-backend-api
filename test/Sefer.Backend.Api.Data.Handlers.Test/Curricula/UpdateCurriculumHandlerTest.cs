namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class UpdateCurriculumHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_CurriculumNull()
    {
        var data = new Curriculum();
        var provider = GetServiceProvider(new Exception());
        
        var updated = await Handle(provider, data);
        
        Assert.IsFalse(updated);
    }

    [TestMethod]
    public async Task Handle_CurriculumNotEditable()
    {
        var closed = new CurriculumRevision {Stage = Stages.Closed};
        var curriculum = new Curriculum {Revisions = new List<CurriculumRevision> {closed}};
        var provider = GetServiceProvider(new Exception());
        provider.AddRequestResult<GetCurriculumByIdRequest, Curriculum>(curriculum);
        
        var updated = await Handle(provider, curriculum);
        
        Assert.IsFalse(updated);
    }
    
    [TestMethod]
    public async Task Handle_Exception()
    {
        var curriculum = new Curriculum {Revisions = new List<CurriculumRevision>()};
        var provider = GetServiceProvider(new Exception());
        provider.AddRequestResult<GetCurriculumByIdRequest, Curriculum>(curriculum);

        var updated = await Handle(provider, curriculum);
        
        Assert.IsFalse(updated);
    }
    
    [TestMethod]
    public async Task Handle_Invalid()
    {
        var curriculum = new Curriculum {Revisions = new List<CurriculumRevision>()};
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCurriculumByIdRequest, Curriculum>(curriculum);

        var updated = await Handle(provider, curriculum);
        
        Assert.IsFalse(updated);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var curriculum = new Curriculum
        {
            Revisions = new List<CurriculumRevision>(),
            Name = "Name",
            Description = "Description",
            Level = Levels.Advanced
        };
        await InsertAsync(curriculum);
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCurriculumByIdRequest, Curriculum>(curriculum);
        
        var updated = await Handle(provider, curriculum);
        
        Assert.IsTrue(updated);
    }

    private static Task<bool> Handle(MockedServiceProvider provider, Curriculum curriculum)
    {
        var request = new UpdateCurriculumRequest(curriculum);
        var handler = new UpdateCurriculumHandler(provider.Object);
        return handler.Handle(request, CancellationToken.None);
    }
}