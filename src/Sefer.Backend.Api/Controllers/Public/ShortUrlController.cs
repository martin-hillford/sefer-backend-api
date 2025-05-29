namespace Sefer.Backend.Api.Controllers.Public;

public class ShortUrlController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IShortUrlService _shortUrlService = serviceProvider.GetService<IShortUrlService>();

    [HttpGet("/short-url-redirect/{reference}")]
    public async Task<ActionResult> RedirectToDestination(string reference)
    {
        var destination = await _shortUrlService.GetDestination(reference);
        if (destination == null) return NotFound();
        return Redirect(destination);
    }

    [HttpGet("/short-url-json/{reference}")]
    public async Task<ActionResult> RedirectFromReference(string reference)
    {
        var destination = await _shortUrlService.GetDestination(reference);
        if (destination == null) return NotFound();
        return Json(new { destination });
    }
}