namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetPublishedCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedCurriculumRevisionRequest, CurriculumRevision>(serviceProvider)
{
    public override Task<CurriculumRevision> Handle(GetPublishedCurriculumRevisionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var helper = new GetPublishedCurriculumRevisionHelper(context);
        var revision = helper.Get(request.CurriculumId);
        return Task.FromResult(revision);
    }
}