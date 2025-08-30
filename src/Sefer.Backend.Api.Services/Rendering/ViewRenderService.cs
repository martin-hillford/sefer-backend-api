using Sefer.Backend.Api.Services.HttpConnection;
using Random = Sefer.Backend.Authentication.Lib.Cryptography.Random;
using Hashing = Sefer.Backend.Authentication.Lib.Cryptography.Hashing;

namespace Sefer.Backend.Api.Services.Rendering;

/// <summary>
/// This class is capable of rendering a razor view to a string.
/// Useful from e-mail and or other applications when a raw HTML string is required
/// </summary>
public class ViewRenderService(IServiceProvider serviceProvider) : IViewRenderService
{
    private readonly IHttpClient _httpClient = serviceProvider.GetRequiredService<IHttpClient>();
        
    private readonly IMediator _mediator = serviceProvider.GetRequiredService<IMediator>();
    
    private readonly RenderConfigurationOptions _options =
        serviceProvider.GetRequiredService<IOptions<RenderConfigurationOptions>>().Value;

    /// <inheritdoc cref="IViewRenderService.RenderToStringAsync{T}(string,string,string,T)"/>>
    public async Task<Render> RenderToStringAsync<T>(string viewName, string language, string type, T data)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ViewRenderService>>();
        return await RenderToStringAsync(viewName, language, type, data, logger);
    }
    
    /// <inheritdoc cref="IViewRenderService.RenderToStringAsync{T}(string,string,string,T,ILogger)"/>>
    public async Task<Render> RenderToStringAsync<T>(string viewName, string language, string type, T data, ILogger logger)
    {
        try
        {
            // First, get the view from the database
            logger.LogDebug("Loading template {ViewName} for type {type}", viewName, type);
            var template = await _mediator.Send(new GetTemplateByNameRequest(viewName, language, type));
            var layout = string.Empty;
            
            // If the view has a layout, load the layout from the database
            if (template.HasLayout)
            {
                logger.LogDebug("Loading layout for template {viewName}", viewName);
                var layoutTemplate = await _mediator.Send(new GetTemplateByNameRequest(template.LayoutName, language, type));
                layout = layoutTemplate?.Content;
            }
        
            // Generate the access token
            var random = Random.GetString(32);
            var accessToken = random + Hashing.Sha256(random + _options.ApiKey);

            // Create the post-body and call the render service
            var body = new { accessToken, template = template.Content, layout, data };
            var requestUri = _options.RenderServiceUrl + "/render";
            
            logger.LogDebug("Calling render service with url {requestUri}", requestUri);
            var response = await _httpClient.PostAsJsonAsync(requestUri, body);
            
            logger.LogDebug("Render service returned with status code {statusCode}", response.StatusCode);
            if(!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
            
            var content = await response.Content.ReadAsStringAsync();
            logger.LogDebug("Rendered template {ViewName} to string with content {content}", viewName, content);

            return new Render { Content = content, Title = template.Title };
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Could not render template {ViewName}", viewName);
            throw;
        }
    }
}