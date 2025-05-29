namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class IsContentPageNameUniqueRequest(int? contentPageId, string name) : IRequest<bool>
{
    public readonly int? ContentPageId = contentPageId;

    public readonly string Name = name;
}