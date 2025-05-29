namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class DeleteCurriculumBlockCourseHandlerTest
    : DeleteEntityHandlerTest<DeleteCurriculumBlockCourseRequest, DeleteCurriculumBlockCourseHandler, CurriculumBlockCourse>
{
    [TestInitialize]
    public async Task Initialize()
    {
        var context = GetDataContext();
        await CurriculumUnitTest.InitializeCurriculumBlockCourse(context);
    }

    protected override async Task<List<(CurriculumBlockCourse Entity, bool IsValid)>> GetTestData()
    {
        var context = GetDataContext();
        var block = await context.CurriculumBlockCourses.FirstAsync();

        return
        [
            (block, true),
            (new CurriculumBlockCourse(), false)
        ];
    }
}