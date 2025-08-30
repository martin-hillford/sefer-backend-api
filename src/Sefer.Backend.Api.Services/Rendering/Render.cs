namespace Sefer.Backend.Api.Services.Rendering;

public class Render
{
    public string Content { get; init;  }
    
    public string Title { get; init;  }

    public string GetSiteTitle(string name) => Title?.Replace("@site", name);
}