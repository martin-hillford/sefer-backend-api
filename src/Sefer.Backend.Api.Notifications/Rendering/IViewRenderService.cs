namespace Sefer.Backend.Api.Notifications.Rendering;

/// <summary>
/// The interface that defines the render view service that is capable of rendering to a string
/// </summary>
public interface IViewRenderService
{
    /// <summary>
    /// This method rendering the view
    /// </summary>
    /// <param name="viewName">The name of the view (following default dotnet core aspnet rules for the location)</param>
    /// <param name="data">The data model to render</param>
    /// <param name="language">The language to render the content in</param>
    /// <returns>A string with the view</returns>
    Task<Render> RenderToStringAsync(string viewName, string language, object data);
}