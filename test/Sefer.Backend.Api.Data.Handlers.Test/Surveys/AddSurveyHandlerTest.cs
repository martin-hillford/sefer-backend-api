namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class AddSurveyHandlerTest : AddEntityHandlerTest<AddSurveyRequest, AddSurveyHandler, Survey>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course = new Course {Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);
        await InsertAsync(new CourseRevision {CourseId = course.Id, Stage = Stages.Edit, Version = 1});
    }
    
    protected override List<(Survey Entity, bool Valid)> GetTestData()
    {
        return
        [
            (new Survey { CourseRevisionId = 1, EnableSurvey = true }, true)
        ];
    }
}