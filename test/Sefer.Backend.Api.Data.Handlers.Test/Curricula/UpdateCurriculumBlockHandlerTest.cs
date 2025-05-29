namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class UpdateCurriculumBlockHandlerTest :
    UpdateEntityHandlerTest<UpdateCurriculumBlockRequest, UpdateCurriculumBlockHandler, CurriculumBlock>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await CurriculumUnitTest.InitializeCurriculumBlock(context);
    }

    protected override async Task<List<(CurriculumBlock entity, bool valid)>> GetTestData()
    {
        var context = GetDataContext();
        var revision = await context.CurriculumRevisions.SingleAsync();
        var block = await context.CurriculumBlocks.SingleAsync();

        return
        [
            (new CurriculumBlock(), false),
            (new CurriculumBlock { CurriculumRevisionId = revision.Id }, false),
            (new CurriculumBlock { CurriculumRevisionId = revision.Id, Id = block.Id }, false),
            (new CurriculumBlock { CurriculumRevisionId = revision.Id, Name = "name" }, false),
            (new CurriculumBlock { CurriculumRevisionId = revision.Id, Name = "name", Id = block.Id }, true)
        ];
    }
}