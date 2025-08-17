using Microsoft.Extensions.Logging;
using Sefer.Backend.Api.Notifications.Rendering;

namespace Sefer.Backend.Api.Data;

public class PdfView(IServiceProvider serviceProvider)
{
    private readonly IViewRenderService _renderService = serviceProvider.GetService<IViewRenderService>();

    private readonly IHttpClientFactory _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

    private readonly PdfOptions _pdfOptions = serviceProvider.GetService<IOptions<PdfOptions>>().Value;
    
    private readonly ILogger<PdfView> _logger = serviceProvider.GetService<ILogger<PdfView>>();

    public async Task<ActionResult> Render(HttpContext context, string view, string language, object model, string fileName)
    {
        try
        {
            var result = await RenderToStringAsync(view, language, model);
            if(result == null) return new StatusCodeResult(418);
            
            var client = _httpClientFactory.CreateClient();
            var body = new { apiKey = _pdfOptions.ApiKey, html = result.Content, fileName };
            var response = await client.PostAsJsonAsync(_pdfOptions.Service + "/generate", body);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error while rendering pdf - could not connect with pdf service");
                return new StatusCodeResult(500);
            }

            context.Response.Headers.CacheControl = "private, max-age=31536000";
            var stream = await response.Content.ReadAsStreamAsync();
            return new FileStreamResult(stream, "application/pdf");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while rendering pdf - could not connect with pdf service.");
            return new StatusCodeResult(500);
        }
    }

    public async Task<ActionResult> RenderAsHtml(string view, string language,  object model)
    {
        var render = await RenderToStringAsync(view, language, model);
        return new ContentResult { Content = render.Content, ContentType = "text/html", StatusCode = 200 };
    }

    private async Task<Render> RenderToStringAsync(string view, string language,  object model)
    {
        try
        {
            return await _renderService.RenderToStringAsync(view, language, "html", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while rendering pdf - could not connect with render service.");
            return null;
        }  
    }
}