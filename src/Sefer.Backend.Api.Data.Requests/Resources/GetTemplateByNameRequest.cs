namespace Sefer.Backend.Api.Data.Requests.Resources;

public class GetTemplateByNameRequest(string name, string language, string type) : IRequest<Template>
{
    public readonly string Name = name?.ToLower();

    public readonly string Language = language?.ToLower();
    
    public string GetTemplateType()
    {
        return type switch
        {
            "text" => "text",
            "xt" => "text",
            "html" => "html",
            _ => "undefined"
        };
    }
}