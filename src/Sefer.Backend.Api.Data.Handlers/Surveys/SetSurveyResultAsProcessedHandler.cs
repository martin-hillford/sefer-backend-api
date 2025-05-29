namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class SetSurveyResultAsProcessedHandler(IServiceProvider serviceProvider)
    : Handler<SetSurveyResultAsProcessedRequest, bool>(serviceProvider)
{
    public override Task<bool> Handle(SetSurveyResultAsProcessedRequest request, CancellationToken token)
    {
        var result = Handle(request);
        return Task.FromResult(result);
    }

    private bool Handle(SetSurveyResultAsProcessedRequest request)
    {
        using var context = GetDataContext();
        var result = context.SurveyResults.SingleOrDefault(r => r.Id == request.SurveyResultId);
        if (result == null) return false;

        result.AdminProcessed = true;
        context.SaveChanges();

        return true;
    }
}