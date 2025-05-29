namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class SetSurveyResultAsProcessedHandlerTest : SurveyResultTest
{
    [TestMethod]
    public async Task Handle_NoResult() => await Handle(19, false);

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var results = await context.SurveyResults.ToListAsync();

        foreach (var result in results)
        {
            await Handle(result.Id, true);
        }
    }

    private async Task Handle(int surveyResultId, bool expected)
    {
        var request = new SetSurveyResultAsProcessedRequest(surveyResultId);
        var handler = new SetSurveyResultAsProcessedHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(expected, result);

        if (!expected) return;

        var context = GetDataContext();
        var surveyResult = await context.SurveyResults.SingleOrDefaultAsync(s => s.Id == surveyResultId);

        Assert.IsNotNull(surveyResult);
        Assert.IsTrue(surveyResult.AdminProcessed);
    }
}