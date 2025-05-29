namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class DeleteContentPageRequest(int id) : IRequest<bool>
{
    public readonly int Id = id;
}