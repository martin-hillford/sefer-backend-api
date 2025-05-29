namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class UpdateSurveyHandlerTest : UpdateEntityHandlerTest<UpdateSurveyRequest, UpdateSurveyHandler, Survey>
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1" });
        await InsertAsync(new CourseRevision { CourseId = 1, Stage = Stages.Edit, Version = 1 });
        await InsertAsync(new Survey { EnableSurvey = true, CourseRevisionId = 1 });
    }


    protected override async Task<List<(Survey entity, bool valid)>> GetTestData()
    {
        var context = GetDataContext();
        var survey = await context.Surveys.FirstAsync();

        return
        [
            (new Survey(), false),
            (new Survey { CourseRevisionId = 1 }, false),
            (survey, true)
        ];
    }
}