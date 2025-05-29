namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class CloseCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : Handler<CloseCurriculumRevisionRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<bool> Handle(CloseCurriculumRevisionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var revision = await context.CurriculumRevisions.SingleOrDefaultAsync(r => r.Id == request.CurriculumRevisionId, token);
        if (revision == null) return false;

        var helper = new CloseCurriculumRevisionHelper(_serviceProvider, context);
        var result = helper.Close(revision);

        return result;
    }
}