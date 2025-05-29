namespace Sefer.Backend.Api.Data.Requests.Resources;

public class GetTemplateByNameRequest(string name, string language) : IRequest<Template>
{
    public readonly string Name = name;
    
    public readonly string Language = language;
}