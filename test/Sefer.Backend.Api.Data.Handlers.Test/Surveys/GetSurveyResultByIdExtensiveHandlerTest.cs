namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class GetSurveyResultByIdExtensiveHandlerTest : SurveyResultTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetSurveyResultByIdExtensiveRequest(1);
        var handler = new GetSurveyResultByIdExtensiveHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Enrollment.Mentor);
        Assert.IsNotNull(result.Enrollment.CourseRevision.Course);
        Assert.IsNotNull(result.MentorRating);
        Assert.IsNotNull(result.CourseRating);
    }
}