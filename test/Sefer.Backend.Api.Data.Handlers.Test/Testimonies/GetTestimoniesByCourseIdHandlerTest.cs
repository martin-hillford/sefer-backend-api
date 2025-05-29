namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class GetTestimoniesByCourseIdHandlerTest : TestimonyHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var courses = await context.Courses.ToListAsync();
        var handler = new GetTestimoniesByCourseIdHandler(GetServiceProvider().Object);

        foreach (var course in courses)
        {
            var request = new GetTestimoniesByCourseIdRequest(course.Id);
            var testimonies = await handler.Handle(request, CancellationToken.None);
            var expected = course.Name == "course.2" ? 2 : 1;
            Assert.AreEqual(expected, testimonies.Count);
        }
    }
}