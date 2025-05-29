namespace Sefer.Backend.Api.Data.Handlers.Test.Testimonies;

[TestClass]
public class SearchTestimoniesHandlerTest : TestimonyHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new SearchTestimoniesRequest("course.2");
        var handler = new SearchTestimoniesHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(2, result.Count);
    }
}