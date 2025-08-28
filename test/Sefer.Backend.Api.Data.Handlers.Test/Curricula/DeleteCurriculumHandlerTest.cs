namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class DeleteCurriculumHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_CurriculumNull()
    {
        var curriculum = new Curriculum { Name = "example" , Description = "Description" };
        var provider = GetServiceProvider();
        
        var updated = await Handle(provider, curriculum);
        
        Assert.IsFalse(updated);
    }

    [TestMethod]
    public async Task Handle_CurriculumNotEditable()
    {
        var curriculum = new Curriculum { Name = "example" , Description = "Description" };
        await InsertAsync(curriculum);
        
        var revision = new CurriculumRevision { Stage = Stages.Published, CurriculumId = curriculum.Id };
        await InsertAsync(revision);
        
        var provider = GetServiceProvider();
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
        var provider = GetServiceProvider();
        provider.AddRequestResult<GetCurriculumByIdRequest, Curriculum>(curriculum);
        await InsertAsync(curriculum);

        var updated = await Handle(provider, curriculum);
        
        Assert.IsTrue(updated);
    }

    private static Task<bool> Handle(MockedServiceProvider provider, Curriculum curriculum)
    {
        var request = new DeleteCurriculumRequest(curriculum);
        var handler = new DeleteCurriculumHandler(provider.Object);
        return handler.Handle(request, CancellationToken.None);
    }
}