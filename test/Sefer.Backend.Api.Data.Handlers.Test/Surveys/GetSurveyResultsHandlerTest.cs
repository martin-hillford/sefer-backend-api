namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class GetSurveyResultsHandlerTest : SurveyResultTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetSurveyResultsRequest(10);
        var handler = new GetSurveyResultsHandler(GetServiceProvider().Object);
        var results = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(2, results.Count);
        
        var result = results.FirstOrDefault();
        Assert.IsNotNull(result);
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Enrollment.Mentor);
        Assert.IsNotNull(result.Enrollment.CourseRevision.Course);
        Assert.IsNotNull(result.MentorRating);
        Assert.IsNotNull(result.CourseRating);
    }
}