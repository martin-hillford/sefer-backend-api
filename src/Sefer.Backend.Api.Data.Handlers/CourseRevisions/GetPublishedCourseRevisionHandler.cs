namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetPublishedCourseRevisionHandler(IServiceProvider serviceProvider)
    : BaseCourseRevisionHandler<GetPublishedCourseRevisionRequest, CourseRevision>(serviceProvider)
{
    public override Task<CourseRevision> Handle(GetPublishedCourseRevisionRequest request, CancellationToken _)
    {
        using var context = GetDataContext();
        var revision = GetPublishedCourseRevision(context, request.CourseId);
        return Task.FromResult(revision);
    }
}