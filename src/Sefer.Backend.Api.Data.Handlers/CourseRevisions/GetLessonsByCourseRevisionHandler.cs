namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetLessonsByCourseRevisionHandler(IServiceProvider serviceProvider)
    : BaseCourseRevisionHandler<GetLessonsByCourseRevisionRequest, List<Lesson>>(serviceProvider)
{
    public override async Task<List<Lesson>> Handle(GetLessonsByCourseRevisionRequest request, CancellationToken token)
    {
        var revision = await Send(new GetCourseRevisionByIdRequest(request.CourseRevisionId), token);
        if (revision == null) return new List<Lesson>();

        await using var context = GetDataContext();
        var lessons = GetLessonsByRevisionId(context, request.CourseRevisionId);
        return lessons;
    }
}