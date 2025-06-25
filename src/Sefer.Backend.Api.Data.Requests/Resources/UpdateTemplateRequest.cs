namespace Sefer.Backend.Api.Data.Requests.Resources;

public class UpdateTemplateRequest(int templateId, string content, string title) : IRequest<bool>
{
    public readonly int TemplateId =  templateId;
    
    public readonly string Content =  content;
    
    public readonly string Title =  title;
}