namespace Sefer.Backend.Api.Models.Admin.Resources;

public class RenderTemplatePostModel
{
    public dynamic Data { get; set; }
    
    public string Name { get; set; }
    
    public string Language { get; set; }
 
    public string Type { get; set; }
}