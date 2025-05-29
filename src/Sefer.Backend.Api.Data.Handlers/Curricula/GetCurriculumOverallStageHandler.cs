namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumOverallStageHandler(IServiceProvider serviceProvider)
    : Handler<GetCurriculumOverallStageRequest, Stages?>(serviceProvider)
{
    public override async Task<Stages?> Handle(GetCurriculumOverallStageRequest request, CancellationToken token)
    {
        var published = await Send(new GetPublishedCurriculumRevisionRequest(request.CurriculumId), token);
        if (published != null) return Stages.Published;

        var closed = await Send(new GetClosedCurriculumRevisionsRequest(request.CurriculumId), token);
        if (closed.Count != 0) return Stages.Closed;

        var editing = await Send(new GetEditingCurriculumRevisionRequest(request.CurriculumId), token);
        return editing?.Stage;
    }
}