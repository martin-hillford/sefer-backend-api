namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumRevisionByIdHandlerTest
    : GetEntityByIdHandlerTest<GetCurriculumRevisionByIdRequest, GetCurriculumRevisionByIdHandler, CurriculumRevision>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced, Permalink = "name" };
        await InsertAsync(curriculum);
    }

    protected override async Task<CurriculumRevision> GetEntity()
    {
        var context = GetDataContext();
        var curriculum = await context.Curricula.SingleAsync();
        return new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
    }
}