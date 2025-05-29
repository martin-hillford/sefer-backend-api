namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCoursesByCurriculumBlockHandler(IServiceProvider serviceProvider)
    : Handler<GetCoursesByCurriculumBlockRequest, List<Course>>(serviceProvider)
{
    public override Task<List<Course>> Handle(GetCoursesByCurriculumBlockRequest request, CancellationToken token)
    {
        using var context = GetDataContext();
        var helper = new GetCoursesByCurriculumBlockHelper(context);
        var courses = helper.Handle(request.CurriculumBlockId, request.PublishedOnly);
        return Task.FromResult(courses);
    }
}