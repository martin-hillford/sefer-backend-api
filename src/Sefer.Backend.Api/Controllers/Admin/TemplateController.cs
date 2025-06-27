using Sefer.Backend.Api.Data.Requests.Resources;
using Sefer.Backend.Api.Models.Admin.Resources;
using Sefer.Backend.Api.Notifications.Rendering;
using Sefer.Backend.Api.Views.Admin.Resources;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class TemplateController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/admin/templates")]
    public async Task<ActionResult> GetTemplates()
    {
        var templates = await Send(new GetTemplatesRequest());
        var views = templates.GroupBy(g => g.Name).Select(g => new TemplateView(g));
        return Json(views);
    }

    [HttpGet("/admin/templates/single")]
    public async Task<ActionResult> GetTemplate([FromQuery] string name, [FromQuery] string language, [FromQuery] string type)
    {
        var template = await Send(new GetTemplateByNameRequest(name, language, type));
        if(template == null) return NotFound();
        return Json(template);
    }

    [HttpPut("/admin/templates/{templateId:int}")]
    public async Task<ActionResult> UpdateTemplate(int templateId, [FromBody] TemplatePostModel template)
    {   
        var saved = await Send(new UpdateTemplateRequest(templateId, template.Content, template.Title));
        return saved ? Accepted() : BadRequest();
    }
    
    [HttpPost("/admin/templates")]
    public async Task<ActionResult> InsertTemplate([FromBody] Template template)
    {
        if (!ModelState.IsValid) return BadRequest();
        var inserted = await Send(new AddTemplateRequest(template));
        return inserted ? Accepted() : BadRequest();
    }

    [HttpPost("/admin/templates/render")]
    public async Task<ActionResult> TryRenderTemplate([FromBody] RenderTemplatePostModel body)
    {
        try
        {
            var template = await Send(new GetTemplateByNameRequest(body.Name, body.Language, body.Type));
            if (template == null) return NotFound();
            var renderService = ServiceProvider.GetService<IViewRenderService>();
            var result = await renderService.RenderToStringAsync(body.Name, body.Language, body.Type, body.Data);
            return Content(result.Content, body.Type == "html" ? "text/html" : "text/plain");
        }
        catch (Exception) { return StatusCode(500); }
    }
}