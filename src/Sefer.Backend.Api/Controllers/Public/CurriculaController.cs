using Sefer.Backend.Api.Data.Requests.Curricula;
using Sefer.Backend.Api.Views.Public.Courses.Curricula;

namespace Sefer.Backend.Api.Controllers.Public;

public class CurriculaController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    [HttpGet("/public/curricula")]
    [ResponseCache(Duration = 86400)]
    [ProducesResponseType(typeof(List<CurriculumView>), 200)]
    public async Task<ActionResult<List<CurriculumView>>> GetAllPublicCurricula()
    {
        var curricula = await Send(new GetPublishedCurriculaRequest(true));
        var view = curricula.Select(c => new CurriculumView(c, _fileStorageService)).ToList();
        return Json(view);
    }
}