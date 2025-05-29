namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class IsCurriculumNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsCurriculumNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsCurriculumNameUniqueRequest request, CancellationToken token)
    {
        var name = request.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return false;
        var context = GetDataContext();
        return !await context.Curricula
            .AnyAsync(sr => sr.Name.ToLower().Trim() == name && sr.Id != request.CurriculumId, token);
    }
}