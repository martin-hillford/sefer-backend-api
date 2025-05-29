namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class IsContentBlockValidRequest(int lessonId, Type contentBlockType, IContentBlock contentBlock)
    : IRequest<bool>
{
    public readonly int LessonId = lessonId;

    public readonly Type ContentBlockType = contentBlockType;

    public readonly IContentBlock ContentBlock = contentBlock;
}