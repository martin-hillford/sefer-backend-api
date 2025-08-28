using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.Admin;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;


namespace Sefer.Backend.Api.Test.Controllers.Admin;

[TestClass]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class CurriculumControllerTest
{
    [TestMethod]
    public async Task DeleteCurriculum_NoCurriculum()
    {
        var services = IntegrationServices.Create();
        var provider = services.BuildServiceProvider();
        var controller = new CurriculumController(provider);
        
        var result = await controller.DeleteCurriculum(13);
        
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public async Task DeleteCurriculum_NotEditable()
    {
        var ( result, services ) = await Delete(Stages.Published);
        Assert.IsInstanceOfType<BadRequestResult>(result);
        Assert.IsNotEmpty(services.Provider.GetContext().Curricula);
        Assert.IsNotEmpty(services.Provider.GetContext().CurriculumRevisions);
    }
    
    [TestMethod]
    public async Task DeleteCurriculum_Success()
    {
        var ( result, services ) = await Delete(Stages.Edit);
        Assert.IsInstanceOfType<NoContentResult>(result);
        Assert.IsEmpty(services.Provider.GetContext().Curricula);
        Assert.IsEmpty(services.Provider.GetContext().CurriculumRevisions);
    }

    private static async Task<(ActionResult, IntegrationServices)> Delete(Stages revisionStage)
    {
        var curriculum = new Curriculum { Name = "example" , Description = "Description" };
        var services = IntegrationServices.Create();
        services.Provider.GetContext().Insert(curriculum);
        var revision = new CurriculumRevision { Stage = revisionStage, CurriculumId = curriculum.Id };
        services.Provider.GetContext().Insert(revision);
        var provider = services.BuildServiceProvider();
        var controller = new CurriculumController(provider);

        return (await controller.DeleteCurriculum(curriculum.Id), services);
    }
}