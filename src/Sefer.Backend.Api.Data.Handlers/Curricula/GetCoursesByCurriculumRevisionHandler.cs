namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCoursesByCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetCoursesByCurriculumRevisionRequest, List<Course>>(serviceProvider)
{
    public override async Task<List<Course>> Handle(GetCoursesByCurriculumRevisionRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.CurriculumBlockCourses
            .Where(c => c.Block.CurriculumRevisionId == request.CurriculumRevisionId)
            .Select(c => c.Course)
            .Distinct()
            .ToListAsync(token);
    }
}