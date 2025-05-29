namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class IsCourseNameUniqueHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await InsertAsync(course1);
    }

    [TestMethod]
    [DataRow("course.1", false)]
    [DataRow("testA", true)]
    [DataRow(null, true)]
    [DataRow(" ", true)]
    [DataRow("", true)]
    public async Task Handle(string name, bool expectUnique)
    {
        var request = new IsCourseNameUniqueRequest(null, name);
        var handler = new IsCourseNameUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expectUnique, unique);
    }

    [TestMethod]
    public async Task Handle_WithSameCourse()
    {
        var context = GetDataContext();
        var first = await context.Courses.FirstOrDefaultAsync();
        Assert.IsNotNull(first);

        var request = new IsCourseNameUniqueRequest(first.Id, first.Name);
        var handler = new IsCourseNameUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.IsTrue(unique);
    }
}