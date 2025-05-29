namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class GetCurriculumBlockByIdHandlerTest
    : GetEntityByIdHandlerTest<GetCurriculumBlockByIdRequest, GetCurriculumBlockByIdHandler, CurriculumBlock>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var curriculum = new Curriculum { Name = "Name", Description = "Description", Level = Levels.Advanced, Permalink = "name" };
        await InsertAsync(curriculum);

        var revision = new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit };
        await InsertAsync(revision);
    }

    protected override async Task<CurriculumBlock> GetEntity()
    {
        var context = GetDataContext();
        var revision = await context.CurriculumRevisions.SingleAsync();
        return new CurriculumBlock { Name = "block", Description = "desc", CurriculumRevisionId = revision.Id };
    }
}