namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class GetUnprocessedSurveyResultsHandlerTest : SurveyResultTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetUnprocessedSurveyResultsRequest(10);
        var handler = new GetUnprocessedSurveyResultsHandler(GetServiceProvider().Object);
        var results = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(1, results.Count);
        
        var result = results.FirstOrDefault();
        Assert.IsNotNull(result);
        Assert.AreEqual("text2",result.Text);
        Assert.IsNotNull(result.Enrollment.Mentor);
        Assert.IsNotNull(result.Enrollment.CourseRevision.Course);
        Assert.IsNotNull(result.MentorRating);
        Assert.IsNotNull(result.CourseRating);
    }
}