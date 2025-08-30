namespace Sefer.Backend.Api.Services.Pdf;

public interface IPdfRenderService
{
    public Task<ActionResult> Render(string view, string language, object model, string fileName);

    public Task<ActionResult> RenderAsHtml(string view, string language, object model);
}