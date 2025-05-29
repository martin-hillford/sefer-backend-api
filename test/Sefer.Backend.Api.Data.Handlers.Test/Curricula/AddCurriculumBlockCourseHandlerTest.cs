namespace Sefer.Backend.Api.Data.Handlers.Test.Curricula;

[TestClass]
public class AddCurriculumBlockCourseHandlerTest : CurriculumUnitTest
{
    [TestInitialize]
    public async Task Initialize() => await InitializeCurriculumBlock();

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var course = context.Courses.Single();
        var block = context.CurriculumBlocks.Single();

        var courseBlock = new CurriculumBlockCourse { BlockId = block.Id, CourseId = course.Id, SequenceId = 2 };
        var request = new AddCurriculumBlockCourseRequest(courseBlock);
        var handler = new AddCurriculumBlockCourseHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsTrue(result);

        await using var data = GetContext();
        Assert.AreEqual(1, data.CurriculumBlockCourses.Count());
    }
}