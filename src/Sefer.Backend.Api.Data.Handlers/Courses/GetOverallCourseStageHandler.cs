namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetOverallCourseStageHandler(IServiceProvider serviceProvider)
    : Handler<GetOverallCourseStageRequest, Stages?>(serviceProvider)
{
    public override async Task<Stages?> Handle(GetOverallCourseStageRequest request, CancellationToken token)
    {
        var published = await Send(new GetPublishedCourseRevisionRequest(request.CourseId), token);
        if (published != null) return Stages.Published;

        var closed = await Send(new GetClosedCourseRevisionsRequest(request.CourseId), token);
        if (closed.Any()) return Stages.Closed;

        var editing = await Send(new GetEditingCourseRevisionRequest(request.CourseId), token);
        return editing?.Stage;
    }
}