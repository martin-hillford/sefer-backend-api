using System.IO;
using System.IO.Compression;
using Sefer.Backend.Api.Data.Requests.Resources;
using Sefer.Backend.Api.Models.Admin.Resources;
using Sefer.Backend.Api.Services.Rendering;
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
    
    [HttpGet("/admin/templates/download")]
    public async Task<ActionResult> DownloadTemplates()
    {
        // Get the templates from the database
        var templates = await Send(new GetTemplatesRequest());
        
        // Create a memory stream to hold the archive in memory
        await using var archiveStream = new MemoryStream();
        using (var zip = new ZipArchive(archiveStream, ZipArchiveMode.Create, true, Encoding.UTF8))
        {
            foreach (var template in templates)
            {
                var filename = $"{template.Name}.{template.Language}.ejs.{template.Type}";
                var entry = zip.CreateEntry(filename);

                await using var entryStream = entry.Open();
                var bytes = Encoding.UTF8.GetBytes(template.Content ?? "");
                await entryStream.WriteAsync(bytes);
            }   
        }
        
        // Rewind to the beginning
        archiveStream.Position = 0;
        var zipArchive = archiveStream.ToArray();
        
        // Return the file as a downloadable response
        return File( zipArchive, "application/zip", "templates.zip");
    }
}