namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class CloseCourseRevisionHandler(IServiceProvider serviceProvider)
    : BaseCourseRevisionHandler<CloseCourseRevisionRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(CloseCourseRevisionRequest request, CancellationToken token)
    {
        using var context = GetDataContext();
        var closed = CloseCourseRevision(context, request.CourseRevisionId);
        return Task.FromResult(closed);
    }
}