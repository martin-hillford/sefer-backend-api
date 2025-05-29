namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class DeleteContentBlockRequest(Type contentBlockType, int contentBlockId) : IRequest<bool>
{
    public readonly Type ContentBlockType = contentBlockType;

    public readonly int ContentBlockId = contentBlockId;
}