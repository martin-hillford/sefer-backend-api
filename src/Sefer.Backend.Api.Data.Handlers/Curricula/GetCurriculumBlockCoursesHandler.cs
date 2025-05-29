namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumBlockCoursesHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumBlockCoursesRequest, List<CurriculumBlockCourse>>(serviceProvider)
{
    public override async Task<List<CurriculumBlockCourse>> Handle(GetCurriculumBlockCoursesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var block = await context.CurriculumBlocks.SingleOrDefaultAsync(b => b.Id == request.CurriculumBlockId, token);
        if (block == null) return new List<CurriculumBlockCourse>();
        return await context.CurriculumBlockCourses.AsNoTracking().Where(b => b.BlockId == block.Id).ToListAsync(token);
    }
}