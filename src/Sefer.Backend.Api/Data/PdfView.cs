using Sefer.Backend.Api.Notifications.Rendering;

namespace Sefer.Backend.Api.Data;

public class PdfView(IServiceProvider serviceProvider)
{
    private readonly IViewRenderService _renderService = serviceProvider.GetService<IViewRenderService>();

    private readonly IHttpClientFactory _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

    private readonly PdfOptions _pdfOptions = serviceProvider.GetService<IOptions<PdfOptions>>().Value;

    public async Task<ActionResult> Render(HttpContext context, string view, string language, object model, string fileName)
    {
        var html = await _renderService.RenderToStringAsync(view, language, model);
        var client = _httpClientFactory.CreateClient();

        var body = new { apiKey = _pdfOptions.ApiKey, html, fileName };
        var response = await client.PostAsJsonAsync(_pdfOptions.Service + "/generate", body);

        if (!response.IsSuccessStatusCode) return new StatusCodeResult(500);

        context.Response.Headers.CacheControl = "private, max-age=31536000";
        var stream = await response.Content.ReadAsStreamAsync();
        return new FileStreamResult(stream, "application/pdf");
    }

    public async Task<ActionResult> RenderAsHtml(string view, string language,  object model)
    {
        var render = await _renderService.RenderToStringAsync(view, language, model);
        return new ContentResult { Content = render.Content, ContentType = "text/html", StatusCode = 200 };
    }
}
