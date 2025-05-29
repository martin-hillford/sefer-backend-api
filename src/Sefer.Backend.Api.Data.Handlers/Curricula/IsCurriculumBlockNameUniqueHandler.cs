namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class IsCurriculumBlockNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsCurriculumBlockNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsCurriculumBlockNameUniqueRequest request, CancellationToken token)
    {
        var name = request.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return false;

        var context = GetDataContext();
        var block = await context.CurriculumBlocks
            .AsNoTracking()
            .Where(b =>
                b.Year == request.Year &&
                b.CurriculumRevision.CurriculumId == request.CurriculumId &&
                b.Name.ToLower().Trim() == name &&
                b.CurriculumRevision.Stage == Stages.Edit
            )
            .FirstOrDefaultAsync(token);

        return block == null || block.Id == request.BlockId;
    }
}