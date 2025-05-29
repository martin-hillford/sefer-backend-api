namespace Sefer.Backend.Api.Data.Handlers.Test.Surveys;

[TestClass]
public class GetSurveyByIdHandlerTest : GetEntityByIdHandlerTest<GetSurveyByIdRequest, GetSurveyByIdHandler, Survey>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course);
        await InsertAsync(new CourseRevision { CourseId = course.Id, Stage = Stages.Edit, Version = 1 });
    }

    protected override async Task<Survey> GetEntity()
    {
        var context = GetDataContext();
        var revision = await context.CourseRevisions.SingleAsync();
        return new Survey
        {
            CreationDate = DateTime.Now,
            EnableMentorRating = true,
            CourseRevisionId = revision.Id
        };
    }
}