namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumByPermalinkHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumByPermalinkRequest, Curriculum>(serviceProvider)
{
    public override async Task<Curriculum> Handle(GetCurriculumByPermalinkRequest request, CancellationToken token)
    {
        var permalink = request?.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return null;

        await using var context = GetDataContext();
        return await context.Curricula
            .AsNoTracking()
            .FirstOrDefaultAsync(curriculum => curriculum.Permalink.ToLower() == permalink, token);
    }
}