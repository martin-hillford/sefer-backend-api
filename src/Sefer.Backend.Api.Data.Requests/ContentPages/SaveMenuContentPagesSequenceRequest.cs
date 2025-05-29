namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class SaveMenuContentPagesSequenceRequest(List<ContentPage> contentPages) : IRequest<bool>
{
    public readonly List<ContentPage> ContentPages = contentPages;
}