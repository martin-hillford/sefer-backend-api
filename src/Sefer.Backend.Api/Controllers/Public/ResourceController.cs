using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Views.Public.Resources;

namespace Sefer.Backend.Api.Controllers.Public;

public class ResourceController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/content/{site}/page/{permalink}")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<PageView>> GetPage(string permalink, string site)
    {
        var page = await Send(new GetPublishedContentPageByPermalinkRequest(permalink, site));
        if (page == null) return NotFound();
        return Json(new PageView(page));
    }

    [HttpGet("/content/{site}/special-page/{type}")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<PageView>> GetSpecialPage(string type, string site)
    {
        try
        {
            var contentType = (ContentPageType)Enum.Parse(typeof(ContentPageType), type, true);
            var page = await Send(new GetPublishedSpecialPageByTypeRequest(contentType, site));
            if (page == null) return NotFound();
            return Json(new PageView(page));
        }
        catch { return NotFound(); }
    }

    [HttpGet("/content/audio/{audioReferenceId:guid}/{fileName}")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public ActionResult GetAudioFile(Guid audioReferenceId, string fileName)
    {
        var audioStorage = ServiceProvider.GetService<IAudioStorageService>();
        if(audioStorage == null) return NotFound();
        return audioStorage.GetAudioFile(audioReferenceId, fileName);
    }
}