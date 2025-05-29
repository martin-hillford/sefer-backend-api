using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Models.Admin.Resources;
using Sefer.Backend.Api.Views.Admin.Resources;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ContentPageController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/admin/content/pages")]
    public async Task<ActionResult<List<ContentPageView>>> GetPages()
    {
        var pages = await Send(new GetContentPagesRequest());
        var view = pages.Select(p => new ContentPageView(p)).ToList();
        return Json(view);
    }

    [HttpGet("/admin/content/pages/{id:int}")]
    public async Task<ActionResult<ContentPageView>> GetPage(int id)
    {
        var page = await Send(new GetContentPageByIdRequest(id));
        if (page == null) return NotFound();
        var view = new ContentPageView(page);
        return Json(view);
    }

    [HttpPost("/admin/content/pages")]
    public async Task<ActionResult<ContentPageView>> Insert([FromBody] ContentPagePostModel model)
    {
        // Validate the model
        if (model == null) return BadRequest();
        var contentPage = ContentPagePostModelFactory.Create(model);
        contentPage.SequenceId = await Send(new GetContentPagesCountRequest());
        if (!await Send(new IsContentPageValidRequest(contentPage))) return BadRequest();

        // Insert it into the database
        var added = await Send(new AddContentPageRequest(contentPage));
        if (added == false) return StatusCode(500);

        // Return the result to the user
        var view = new ContentPageView(contentPage);
        return Created($"/admin/content/pages/{view.Id}", view);
    }

    [HttpPut("/admin/content/pages/{id:int}")]
    public async Task<ActionResult> Update([FromBody] ContentPagePostModel model, int id)
    {
        // Validate the model
        if (model == null) return BadRequest();
        var contentPage = await Send(new GetContentPageByIdRequest(id));
        if (contentPage == null) return NotFound();

        ContentPagePostModelFactory.Patch(contentPage, model);
        if (!await Send(new IsContentPageValidRequest(contentPage))) return BadRequest();

        // Update the database
        var updated = await Send(new UpdateContentPageRequest(contentPage));
        return updated ? StatusCode(200) : StatusCode(500);
    }

    [HttpPost("/admin/content/site-specific-pages")]
    public async Task<ActionResult> SetSiteSpecificPage([FromBody] SiteSpecificPageModel body)
    {
        // Check if content exists
        if (string.IsNullOrEmpty(body.Content?.Trim())) return BadRequest();
        if (string.IsNullOrEmpty(body.Site?.Trim())) return BadRequest();

        // Create the request
        var request = new SetSiteSpecificPageRequest
        {
            Site = body.Site,
            Content = body.Content,
            ContentPageId = body.ContentPageId,
            IsPublished = body.IsPublished
        };

        // Return the result of the request
        var contentPage = await Send(request);
        return contentPage != null ? Json(contentPage) : BadRequest();
    }

    [HttpDelete("/admin/content/pages/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var contentPage = await Send(new GetContentPageByIdRequest(id));
        if (contentPage == null) return NotFound();

        var deleted = await Send(new DeleteContentPageRequest(contentPage.Id));
        return deleted ? StatusCode(204) : StatusCode(500);
    }

    [HttpPost("/admin/content/pages/sorting")]
    public async Task<ActionResult> SaveSorting([FromBody] List<int> ids)
    {
        var pages = new List<ContentPage>();
        var existing = await Send(new GetContentPagesRequest());
        var lookup = existing.ToDictionary(p => p.Id);

        foreach (var pageId in ids)
        {
            if (lookup.TryGetValue(pageId, out var page) == false) return BadRequest();
            if (page.Type != ContentPageType.MenuPage) return BadRequest();
            pages.Add(page);
        }

        var sorted = await Send(new SaveMenuContentPagesSequenceRequest(pages));
        return sorted ? NoContent() : StatusCode(500);
    }

    [HttpGet("/admin/content/pages/links")]
    public async Task<ActionResult<List<ContentPageLink>>> GetPageLinks()
    {
        var pages = await Send(new GetContentPageLinksRequest());
        return Json(pages);
    }

    [HttpPost("/admin/content/pages/permalink")]
    public async Task<ActionResult<BooleanView>> IsPermalinkUnique([FromBody] IsPermalinkUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var page = await Send(new GetContentPageByPermalinkRequest(post.Permalink));
        var view = new BooleanView(page == null || page.Id == post.Id);
        return Json(view);
    }

    [HttpPost("/admin/content/pages/name")]
    public async Task<IActionResult> IsNameUnique([FromBody] IsNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsContentPageNameUniqueRequest(post.Id, post.Name));
        var view = new BooleanView(isUnique);
        return Json(view);
    }

    [HttpGet("/admin/content/specific-pages/{site}/{contentPageId:int}")]
    public async Task<ActionResult> GetSiteSpecificPage(int contentPageId, string site)
    {
        var page = await Send(new GetSiteSpecificByContentPageIdRequest(contentPageId, site));
        if (page == null) return NotFound();
        return Json(page);
    }
}