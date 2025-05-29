namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class UpdateCurriculumRevisionHandlerTest
    : UpdateEntityHandlerTest<UpdateCurriculumRevisionRequest, UpdateCurriculumRevisionHandler, CurriculumRevision>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await CurriculumUnitTest.InitializeCurriculumBlock(context);
    }

    protected override async Task<List<(CurriculumRevision entity, bool valid)>> GetTestData()
    {
        var context = GetDataContext();
        var curriculum = await context.Curricula.SingleAsync();
        var revision = await context.CurriculumRevisions.SingleAsync();

        return
        [
            (new CurriculumRevision { CurriculumId = -1 }, false),
            (new CurriculumRevision(), false),
            (new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit }, false),
            (new CurriculumRevision { CurriculumId = curriculum.Id, Stage = Stages.Edit, Id = revision.Id }, true)
        ];
    }
}