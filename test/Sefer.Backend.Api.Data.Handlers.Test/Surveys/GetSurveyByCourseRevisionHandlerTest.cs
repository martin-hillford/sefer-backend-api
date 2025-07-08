namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class GetSurveyByCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InsertAsync(new Course {Name = "course.1", Description = "course.1", Permalink = "course1" });
        await InsertAsync(new CourseRevision {CourseId = 1, Stage = Stages.Edit, Version = 1, GeneralInformation = "General" });
        await InsertAsync(new CourseRevision {CourseId = 1, Stage = Stages.Edit, Version = 2, AllowSelfStudy = true});
        await InsertAsync(new Survey { CourseRevisionId = 1, EnableMentorRating = true });
        await InsertAsync(new Survey { CourseRevisionId = 2, EnableMentorRating = true  });
    }

    [TestMethod]
    public async Task Handle_NoCourseRevision()
    {
        var survey = await Handle(19);
        Assert.IsNull(survey);
    }
    
    [TestMethod]
    public async Task Handle_AllowSelfStudy()
    {
        var survey = await Handle(2);
        Assert.IsNotNull(survey);
        Assert.IsFalse(survey.EnableMentorRating);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var survey = await Handle(1);
        Assert.IsNotNull(survey);
        Assert.IsTrue(survey.EnableMentorRating);
    }

    private async Task<Survey> Handle(int courseRevisionId)
    {
        var request = new GetSurveyByCourseRevisionRequest(courseRevisionId);
        var handler = new GetSurveyByCourseRevisionHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}