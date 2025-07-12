using Sefer.Backend.Api.Data.Requests.Resources;
using Sefer.Backend.Api.Services.HttpConnection;
using Random = Sefer.Backend.Authentication.Lib.Cryptography.Random;
using Hashing = Sefer.Backend.Authentication.Lib.Cryptography.Hashing;

namespace Sefer.Backend.Api.Notifications.Rendering;

/// <summary>
/// This class is capable of rendering a razor view to a string.
/// Useful from e-mail and or other applications when a raw HTML string is required
/// </summary>
public class ViewRenderService(IServiceProvider serviceProvider) : IViewRenderService
{
    private readonly IHttpClient _httpClient = serviceProvider.GetRequiredService<IHttpClient>();
        
    private readonly IMediator _mediator = serviceProvider.GetRequiredService<IMediator>();
        
    private readonly ILogger<ViewRenderService> _logger = serviceProvider.GetRequiredService<ILogger<ViewRenderService>>();

    private readonly RenderConfigurationOptions _options =
        serviceProvider.GetRequiredService<IOptions<RenderConfigurationOptions>>().Value;
    
    /// <summary>
    /// This method rendering the view
    /// </summary>
    /// <param name="viewName">The name of the view - as specified in the database </param>
    /// <param name="data">The data model to render</param>
    /// <param name="language">The language to render the content in</param>
    /// <param name="type">The type of the template (text, HTML)</param>
    /// <returns>A string with the view</returns>
    public async Task<Render> RenderToStringAsync<T>(string viewName, string language, string type, T data)
    {
        try
        {
            // First, get the view from the database
            var template = await _mediator.Send(new GetTemplateByNameRequest(viewName, language, type));
            var layout = string.Empty;
            
            // If the view has a layout, load the layout from the database
            if (template.HasLayout)
            {
                var layoutTemplate = await _mediator.Send(new GetTemplateByNameRequest(template.LayoutName, language, type));
                layout = layoutTemplate?.Content;
            }
        
            // Generate the access token
            var random = Random.GetString(32);
            var accessToken = random + Hashing.Sha256(random + _options.ApiKey);

            // Create the post-body and call the render service
            var body = new { accessToken, template = template.Content, layout, data };
            var requestUri = _options.RenderServiceUrl + "/render";
            var response = await _httpClient.PostAsJsonAsync(requestUri, body);
            if(!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
            var content = await response.Content.ReadAsStringAsync();

            return new Render { Content = content, Title = template.Title };
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not render template {ViewName}", viewName);
            throw;
        }
    }
}